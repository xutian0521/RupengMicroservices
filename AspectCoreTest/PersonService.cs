using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspectCoreTest
{
    public class PersonService
    {
        [CustomInterceptorAttribute]
        public virtual string Say()
        {
            string s = null;
            //s.ToArray();
            return "Hellow !";
        }
    }
}
