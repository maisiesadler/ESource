using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ESource.WebSockets
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseWebSockets();

            var manager = new WebSocketConnectionManager();

            app.Use(manager.HandleRequest);

            //app.Use(async (context, next) =>
            //{
            //    var http = (HttpContext)context;

            //    if (http.WebSockets.IsWebSocketRequest)
            //    {
            //        WebSocket webSocket = await http.WebSockets.AcceptWebSocketAsync();
            //        var bytes = Encoding.UTF8.GetBytes("testing132");
            //        await webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, _token);
            //    }
            //});
        }
    }
}
