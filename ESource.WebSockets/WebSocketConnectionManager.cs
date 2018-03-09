using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ESource.WebSockets
{
    public class Model
    {
        public CancellationTokenSource CancellationToken { get; }
        public ReplaySubject<string> Subject { get; }
        public Model(ReplaySubject<string> subject, CancellationTokenSource cancellationToken)
        {
            Subject = subject;
            CancellationToken = cancellationToken;
        }
    }
    
    public class WebSocketConnectionManager
    {
        Dictionary<string, Model> _models = new Dictionary<string, Model>();
        CancellationToken _token = new CancellationTokenSource().Token;
        public WebSocketConnectionManager()
        {
        }

        private Model CreateModel(string name)
        {
            var model = new ReplaySubject<string>();
            var cancellationToken = new CancellationTokenSource();
            Task.Run(async () =>
             {
                 var a = 0;
                 while (!cancellationToken.IsCancellationRequested)
                 {
                     await Task.Delay(1000);
                     model.OnNext(name + a++);
                 }
                 model.OnCompleted();
             }, cancellationToken.Token);

            return new Model(model, cancellationToken);
        }

        private ReplaySubject<string> GetOrCreateModel(string name)
        {
            if (!_models.ContainsKey(name))
            {
                _models[name] = CreateModel(name);
            }
            return _models[name].Subject;
        }

        private void TryCancel(string name)
        {
            if (_models.ContainsKey(name))
            {
                _models[name].CancellationToken.Cancel();
            }
        }

        public async Task HandleRequest(HttpContext context, Func<Task> next)
        {
            var http = (HttpContext)context;

            if (http.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await http.WebSockets.AcceptWebSocketAsync();
                var re = await webSocket.ReceiveAsync(new ArraySegment<byte>(), new CancellationToken());

                var isRequestingModel = http.Request.Headers.TryGetValue("modelname", out var vals);

                if (isRequestingModel)
                {
                    var tcs = new TaskCompletionSource<object>();
                    var subject = GetOrCreateModel(vals);
                    var subscription = subject.Subscribe(async s =>
                    {
                        var c = http;
                        var bytes = Encoding.UTF8.GetBytes(s);
                        await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, _token);
                    }, () => { tcs.SetResult(new object()); });
                    await Task.Delay(3000);
                    TryCancel(vals);
                    await tcs.Task;
                    subscription.Dispose();
                }
            }
        }
    }
}
