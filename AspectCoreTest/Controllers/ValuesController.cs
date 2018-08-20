using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AspectCore;
using AspectCore.DynamicProxy;

namespace AspectCoreTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public PersonService _personService;
        public ValuesController(PersonService personService)
        {
            this._personService= personService;
        }
        // GET api/values
        [HttpGet("Say")]
        [CustomInterceptorAttribute]
        public virtual string Say()
        {
            // ProxyGeneratorBuilder proxyGeneratorBuilder = new ProxyGeneratorBuilder();
            // IProxyGenerator proxyGenerator = proxyGeneratorBuilder.Build();
            // PersonService person = proxyGenerator.CreateClassProxy<PersonService>();
            PersonService person= _personService;
            return person.Say();
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
