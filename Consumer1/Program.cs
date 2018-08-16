using System;
using System.Threading.Tasks;
using Consul;
using System.Net.Http;
using System.Linq;
using System.Text;

namespace Consumer1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using(var consul= new ConsulClient(c => c.Address=new Uri("http://127.0.0.1:8500")))
            {
                var Services= await consul.Agent.Services();
                //foreach (var item in Services.Response.Values)
                //{
                //    Console.WriteLine($"service:{item.Service}, address:{item.Address}, port:{item.Port}");
                //}
                // 选着一台服务器 发送短信
                using (HttpClient httpClient=new HttpClient())
                {
                    var msgService= Services.Response.Values.Where(t => t.Service == "MsgService").FirstOrDefault();
                    var productService = Services.Response.Values.Where(t => t.Service == "ProductService").FirstOrDefault();
                    string api= $"http://{msgService.Address}:{msgService.Port}/api/SMS/Send_MI";
                    string api2= $"http://{productService.Address}:{productService.Port}/api/Product";
                    HttpContent httpContent = new StringContent("{phoneNum:'18711110027',msg: 'hellow msg!'}", Encoding.UTF8, "application/json");
                    HttpResponseMessage reslut =await httpClient.PostAsync(api, httpContent);
                    HttpResponseMessage reslut2 = await httpClient.GetAsync(api2+"?Id=1");
                    Console.WriteLine(reslut.StatusCode + await reslut.Content.ReadAsStringAsync());
                    
                }
            }
            Console.ReadKey();
        }
    }
}
