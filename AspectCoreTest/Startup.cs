using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AspectCore;
using AspectCore.Extensions.DependencyInjection;
using System.Reflection;

namespace AspectCoreTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //在 asp.net core 项目中，可以借助于 asp.net core 的依赖注入，简化代理类对象的注入，不用再自己调用 ProxyGeneratorBuilder 进行代理类对象的注入了。
            //services.AddSingleton<PersonService>();
            //优化：当然要通过反射扫描所有 Service 类，只要类中有标记了 CustomInterceptorAttribute 的方法
            //都算作服务实现类。为了避免一下子扫描所有类，所以 RegisterServices 还是手动指定从哪个程序集中加载。
            RegisterServices(this.GetType().Assembly, services);
            //修改 Startup.cs 的 ConfigureServices 方法，把返回值从 void 改为 IServiceProvider
            return services.BuildAspectCoreServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
        private static void RegisterServices(Assembly asm, IServiceCollection services)
        {
            //遍历程序集中的所有 public 类型
            foreach (Type type in asm.GetExportedTypes())
            {
                //判断类中是否有标注了 CustomInterceptorAttribute 的方法
                bool hasCustomInterceptorAttr = type.GetMethods()
                .Any(m => m.GetCustomAttribute(typeof(CustomInterceptorAttribute)) != null);
                if (hasCustomInterceptorAttr)
                {
                    services.AddSingleton(type);
                }
            }
        }
    }
}
