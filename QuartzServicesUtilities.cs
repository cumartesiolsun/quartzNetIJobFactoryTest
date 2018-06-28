using Quartz;
using System;

namespace QuartzNetJobFactoryTest
{
    public static class QuartzServicesUtilities
    {
        public static void StartJob<TJob>(IScheduler scheduler, TimeSpan runInterval)
        where TJob : IJob
        {
            var jobName = typeof(TJob).FullName;

            var job = JobBuilder.Create<TJob>()
                .WithIdentity(jobName)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity($"{jobName}.trigger")
                .StartNow()
                .WithSimpleSchedule(scheduleBuilder =>
                    scheduleBuilder
                        .WithInterval(runInterval)
                        .RepeatForever())
                .Build();

            scheduler.ScheduleJob(job, trigger);
        }
    }
}