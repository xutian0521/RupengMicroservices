using AspectCore.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspectCoreTest
{
    public class CustomInterceptorAttribute : AbstractInterceptorAttribute
    {
        public override async Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call");
                await next(context);
                Console.WriteLine("After service call");
            }
            catch (Exception ex)
            {

                Console.WriteLine("异常捕捉测试");
            }

        }
    }
}
