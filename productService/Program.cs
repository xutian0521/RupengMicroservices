using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace productService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Run();
        }

        public static IWebHost CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .Build();
            string ip = config["ip"];
            string port = config["port"];
            Console.WriteLine($"ip={ip},port={port}");
            return WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseUrls($"http://{ip}:{port}")
            .Build();
        }
    }
}
