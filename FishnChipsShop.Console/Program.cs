using FishnChipsShop.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FishnChipsShop.Console
{
    class Program
    {
        static void Main(string[] args)
        {
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterServices();
        }
    }
}
