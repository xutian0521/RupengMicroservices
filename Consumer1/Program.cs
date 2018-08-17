using System;
using System.Threading.Tasks;
using Consul;
using System.Net.Http;
using System.Linq;
using System.Text;
using RestTemplateCore;

namespace Consumer1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            //await PrimitiveAsync();
            await RestTemplateCoreAsync();
            Console.ReadKey();
        }
        /// <summary>
        /// 原始的调用方法
        /// </summary>
        static async Task PrimitiveAsync()
        {
            using (var consul = new ConsulClient(c => c.Address = new Uri("http://127.0.0.1:8500")))
            {
                var Services = await consul.Agent.Services();
                //foreach (var item in Services.Response.Values)
                //{
                //    Console.WriteLine($"service:{item.Service}, address:{item.Address}, port:{item.Port}");
                //}
                if (Services.Response.Values.Any())
                {
                    // 选着一台服务器 发送短信
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var msgServices = Services.Response.Values.Where(t => t.Service == "MsgService").ToList();
                        var productServices = Services.Response.Values.Where(t => t.Service == "ProductService").ToList();
                        var msgService = msgServices.ElementAt(Environment.TickCount % msgServices.Count());
                        var productService = productServices.ElementAt(Environment.TickCount % productServices.Count());
                        string api = $"http://{msgService.Address}:{msgService.Port}/api/SMS/Send_MI";
                        string api2 = $"http://{productService.Address}:{productService.Port}/api/Product";
                        HttpContent httpContent = new StringContent("{phoneNum:'18711110027',msg: 'hellow msg!'}", Encoding.UTF8, "application/json");
                        HttpResponseMessage reslut = await httpClient.PostAsync(api, httpContent);
                        HttpResponseMessage reslut2 = await httpClient.GetAsync(api2 + "?Id=1");
                        Console.WriteLine(reslut.StatusCode + await reslut.Content.ReadAsStringAsync());

                    }
                }
                else
                {
                    Console.WriteLine("找不到服务的实例");
                }

            }
        }
        /// <summary>
        /// RestTemplateCore 方式
        /// </summary>
        /// <returns></returns>
        static async Task RestTemplateCoreAsync()
        {
            using (HttpClient httpClient=new HttpClient())
            {
                RestTemplate rest = new RestTemplate(httpClient);
                Console.WriteLine("---查询数据---------");
                var resp1= await rest.GetForEntityAsync<Product[]>("http://ProductService/api/Product");
                Console.WriteLine(resp1.StatusCode);
                if (resp1.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    foreach (var p in resp1.Body)
                    {
                        Console.WriteLine($"id={p.Id},name={p.Name}");
                    }
                }
                Console.WriteLine("---新增数据---------");
                Product newP = new Product();
                newP.Id = 888;
                newP.Name = "辛增";
                newP.Price = 88.8;
                var resp2 = await rest.PostAsync("http://ProductService/api/Product", newP);
                Console.WriteLine(resp2.StatusCode);
            }
        }
    }
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
    }
}
