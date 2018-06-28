using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Net.Http;

namespace QuartzNetJobFactoryTest
{
    public static class ServiceCollectionExtension
    {
        public static void UseQuartz(this IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddScoped<ItJob>();
            var _client = new HttpClient();
            var result = JsonConvert.DeserializeObject<ServerConfigurationResponseModel>(_client.GetStringAsync(ConfigurationKeys.KEY_VAULT_SERVER_ADDRESS).Result);

            services.AddSingleton<ProjServerConfiguration>(options =>
            {
                ProjServerConfiguration ProjServerConfiguration = new ProjServerConfiguration
                {
                    EnvironmentIpAddress = result.RequestorIpAddress,
                    Environment = result.Environment,
                    DatabaseConnectionString = result.Configurations[ConfigurationKeys.DATABASE_CONNECTION_STRING],
                    NoSqlConnectionString = result.Configurations[ConfigurationKeys.NOSQL_CONNECTION_STRING],
                    NoSqlDatabaseName = result.Configurations[ConfigurationKeys.NOSQL_DATABASE_NAME],
                    RedisConfigurationOptions = result.Configurations[ConfigurationKeys.REDIS_CONFIGURATION_OPTIONS],
                    RedisDatabaseNumber = Convert.ToInt32(result.Configurations[ConfigurationKeys.REDIS_DATABASE_NUMBER])
                };
                return ProjServerConfiguration;
            });

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var serviceProvider = services.BuildServiceProvider();
            services.AddDbContext<ProjDataContext>(options =>
            {
                var ProjConfInstance = serviceProvider.GetService<ProjServerConfiguration>();
                options.UseSqlServer(ProjConfInstance.DatabaseConnectionString, b => b.UseRowNumberForPaging());
            });

            services.AddDbContext<IlkDataContext>(); 

            services.AddTransient<ISecurityManager, SecurityManager>();
            services.AddTransient<ICacheManager, CacheManager>();
            services.AddTransient<ILogManager, LogManager>(); 
            //....

            services.AddSingleton(provider =>
            {
                ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
                IScheduler scheduler = schedulerFactory.GetScheduler().Result;
                scheduler.JobFactory = provider.GetService<IJobFactory>();
                scheduler.Start().Wait();

                var job = JobBuilder.Create<ItJob>()
                .WithIdentity("JobBuilderItJob")
                .Build();
                var trigger = TriggerBuilder.Create()
                    .WithIdentity("TriggerBuilderItJob")
                    .Build();

                scheduler.ScheduleJob(job, trigger);

                return scheduler;
            });

            serviceProvider = services.BuildServiceProvider();

            var iCommonLoginWorkflow = serviceProvider.GetService<ICommonLoginWorkflow>();
            var _cacheManager = serviceProvider.GetService<ICacheManager>();

            var dbContext = serviceProvider.GetService<ProjDataContext>();
        }
    }
}