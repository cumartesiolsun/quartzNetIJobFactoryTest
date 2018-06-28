using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;

namespace QuartzNetJobFactoryTest
{
    internal class Program
    {
        public static IServiceProvider _serviceProvider;

        private static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.UseQuartz();

            var serviceProvider = services.BuildServiceProvider();

            var scheduler = serviceProvider.GetService<IScheduler>();
            ConfigureServices(services, scheduler);
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services, IScheduler scheduler)
        {
            QuartzServicesUtilities.StartJob<ItJob>(scheduler, TimeSpan.FromSeconds(5));
        }
    }
}