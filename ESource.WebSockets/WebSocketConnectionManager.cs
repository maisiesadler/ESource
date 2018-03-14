using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESource.WebSockets
{
    public class WebSocketConnectionManager
    {
        CancellationToken _token = new CancellationTokenSource().Token;
        private Backend _backend;
        private ILogger _logger;

        public WebSocketConnectionManager(ILogger logger)
        {
            _backend = new Backend();
            _logger = logger;
        }
        public async Task HandleRequest(HttpContext httpContext, Func<Task> next)
        {
            if (httpContext.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await httpContext.WebSockets.AcceptWebSocketAsync();

                var isRequestingModel = true;
                if (isRequestingModel)
                {
                    var tcs = new TaskCompletionSource<object>();
                    var cancellationTokenSource = new CancellationTokenSource();

                    var obs = _backend.Read();

                    var a = 1;
                    var subscription = obs.Subscribe(s =>
                     {
                         if (a++ == 4)
                             tcs.SetResult(new object());
                         var bytes = Encoding.UTF8.GetBytes(s);
                         webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, _token);
                     }, () =>
                     {
                         var bytes = Encoding.UTF8.GetBytes("Completed");
                         webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, _token);
                         tcs.SetResult(new object());
                     });

                    ReadSocketWithRetry(webSocket, tcs, cancellationTokenSource, subscription);

                    await tcs.Task;
                    subscription?.Dispose();
                    cancellationTokenSource.Cancel();
                }
            }
        }

        private async Task ReadSocketWithRetry(
            WebSocket webSocket,
            TaskCompletionSource<object> taskCompletionSource,
            CancellationTokenSource cancellationTokenSource,
            IDisposable subscription)
        {
            bool connected = true;

            while (connected && !cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    (WebSocketMessageType, JObject) result = await ReadSocket(webSocket, taskCompletionSource, cancellationTokenSource);
                    var messageType = result.Item1;
                    if (messageType == WebSocketMessageType.Text)
                    {
                        var jObject = result.Item2;
                        _backend.Process(jObject);
                    }
                    else if (messageType == WebSocketMessageType.Close)
                    {
                        connected = false;
                        subscription.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogException("Error thrown while reading socket", ex);
                }
            }
        }
        private async Task<(WebSocketMessageType, JObject)> ReadSocket(
            WebSocket webSocket, 
            TaskCompletionSource<object> taskCompletionSource,
            CancellationTokenSource cancellationTokenSource)
        {
            var buffer = WebSocket.CreateClientBuffer(4096, 4096);
            WebSocketReceiveResult result = null;
            JObject jObject = null;

            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, cancellationTokenSource.Token);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        var msg = reader.ReadToEnd();
                        jObject = JsonConvert.DeserializeObject<JObject>(msg);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    taskCompletionSource.SetResult(new object());
                }

                return (result.MessageType, jObject);
            }
        }
    }
}
