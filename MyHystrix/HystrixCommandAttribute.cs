using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyHystrix
{
    public class HystrixCommandAttribute : AbstractInterceptorAttribute
    {
        public HystrixCommandAttribute(string fallbackMethod)
        {
            this.FallbackMethod = fallbackMethod;
        }
        public string FallbackMethod { get; set; }
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {

            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                //context.ServiceMethod 被拦截的方法。context.ServiceMethod.DeclaringType 被拦截方法所在的类
                //context.Implementation 实际执行的对象 p
                //context.Parameters 方法参数值
                MethodInfo fallBackMethod = context.ServiceMethod.DeclaringType.GetMethod(this.FallbackMethod);
                object fallBackResult = fallBackMethod.Invoke(context.Implementation, context.Parameters);
                context.ReturnValue = fallBackResult;

            }

        }
    }
}
