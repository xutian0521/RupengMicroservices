using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyHystrix
{
    public class PersonService
    {
        [HystrixCommand(nameof(SayHelloFallback1))]
        public virtual string SayHello()
        {
            throw new ArgumentNullException("1");
            return "hello!";
        }
        [HystrixCommand(nameof(SayHelloFallback2))]
        public virtual string SayHelloFallback1()
        {
            throw new ArgumentNullException("2");
            return "hello Fallback1!";
        }
        public virtual string SayHelloFallback2()
        {
            return "hello Fallback2!";
        }
    }
}
