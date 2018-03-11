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

        public WebSocketConnectionManager()
        {
            _backend = new Backend();
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
                    ReadSocket(webSocket, tcs);

                    var subject = _backend.Read();
                    var subscription = subject.Subscribe(async s =>
                    {
                        var bytes = Encoding.UTF8.GetBytes(s);
                        await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, _token);
                    }, () => { tcs.SetResult(new object()); });

                    await tcs.Task;
                    subscription.Dispose();
                }
            }
        }

        private async Task ReadSocket(WebSocket webSocket, TaskCompletionSource<object> taskCompletionSource)
        {
            var buffer = WebSocket.CreateClientBuffer(4096, 4096);
            WebSocketReceiveResult result = null;

            using (var ms = new MemoryStream())
            {
                do
                {
                    result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                    {
                        // do stuff
                        var msg = reader.ReadToEnd();
                        JObject jObject = JsonConvert.DeserializeObject<JObject>(msg);
                        _backend.Process(jObject);
                    }
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    taskCompletionSource.SetResult(new object());
                }
            }
        }
    }
}
