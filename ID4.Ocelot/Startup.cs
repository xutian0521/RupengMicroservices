using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ID4.Ocelot
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //指定Identity Server的信息
            Action<IdentityServerAuthenticationOptions> isaOptMsg = o => {
                o.Authority = "http://127.0.0.1:9500";
                o.ApiName = "MsgAPI";//要连接的应用的名字
                o.RequireHttpsMetadata = false;
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "123321";//秘钥
            };
            Action<IdentityServerAuthenticationOptions> isaOptProduct = o => {
                o.Authority = "http://127.0.0.1:9500";
                o.ApiName = "ProductAPI";//要连接的应用的名字
                o.RequireHttpsMetadata = false;
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "123321";//秘钥
            };
            services.AddAuthentication()
            //对配置文件中使用ChatKey配置了AuthenticationProviderKey=MsgKey
            //的路由规则使用如下的验证方式
            .AddIdentityServerAuthentication("MsgKey", isaOptMsg)
            .AddIdentityServerAuthentication("ProductKey", isaOptProduct);
            services.AddOcelot();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseOcelot().Wait();
            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
