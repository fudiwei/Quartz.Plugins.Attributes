using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Logging;

namespace Quartz
{
    using Plugins.Attributes;

    /// <summary>
    /// 
    /// </summary>
    public static class ApplicationBuilderQuartzJobsExtensions
    {
        private class QuartaJobMeta
        {
            public IJobDetail JobDetail { get; }

            public ITrigger Trigger { get; }

            public QuartaJobMeta(IJobDetail jobDetail, ITrigger trigger)
            {
                JobDetail = jobDetail;
                Trigger = trigger;
            }
        }

        /// <summary>
        /// Start to schedule all Quartz.NET jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseQuartzJobs(this IApplicationBuilder app)
        {
            IServiceProvider provider = app.ApplicationServices;

            // SetUp Logger
            ILoggerFactory loggerFactory = provider.GetService<ILoggerFactory>();
            ILogger logger = loggerFactory?.CreateLogger("Quartz.NET");
            LogProvider.IsDisabled = true;
            LogProvider.SetCurrentLogProvider(new QuartzLogProvider(loggerFactory));

            // SetUp Jobs & Triggers
            IList<QuartaJobMeta> jobs = new List<QuartaJobMeta>();
            ISchedulerFactory schedulerFactory = provider.GetRequiredService<ISchedulerFactory>();
            IScheduler scheduler = schedulerFactory?.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
            QuartzJobTypeContainer container = provider.GetService<QuartzJobTypeContainer>();
            if (container != null)
            {
                foreach (Type type in container)
                {
                    QuartzJobAttribute attribute = type.GetCustomAttributes(typeof(QuartzJobAttribute), true).FirstOrDefault() as QuartzJobAttribute;
                    if (attribute == null)
                    {
                        logger?.LogWarning("QuartzJobAttribute is not found in class \"{0}\".", type.FullName);
                        continue;
                    }

                    string jobName = string.IsNullOrEmpty(attribute.Name) ? type.Name : attribute.Name;
                    IJobDetail jobDetail = JobBuilder.Create()
                        .OfType(type)
                        .WithIdentity(jobName, attribute.Group)
                        .RequestRecovery(attribute.RequestRecovery)
                        .StoreDurably(attribute.StoreDurably)
                        .Build();
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity(jobName + "Trigger", attribute.Group)
                        .WithPriority(attribute.Priority)
                        .WithCronSchedule(attribute.CronExpression, builder => builder.InTimeZone(TimeZoneInfo.Local))
                        .ForJob(jobDetail)
                        .StartNow()
                        .Build();

                    jobs.Add(new QuartaJobMeta(jobDetail, trigger));
                }
            }

#if NETCORE_2_X
            IApplicationLifetime lifetime = provider.GetService<IApplicationLifetime>();
#else
            Microsoft.Extensions.Hosting.IHostApplicationLifetime lifetime = provider.GetService<Microsoft.Extensions.Hosting.IHostApplicationLifetime>();
#endif
            if (lifetime != null)
            {
                // Follow the LifeCycle of ASP.NET Core
                lifetime.ApplicationStarted.Register(async () =>
                {
                    await Task.WhenAll(jobs.Select(e =>
                    {
                        logger?.LogInformation("Ready to schedule job \"{0}\".", e.JobDetail.Key.Name);
                        return scheduler.ScheduleJob(e.JobDetail, e.Trigger);
                    }));
                    await scheduler.Start();
                });
                lifetime.ApplicationStopping.Register(async () =>
                {
                    if (!scheduler.IsShutdown)
                    {
                        await scheduler.Shutdown(false);
                    }
                });
            }
            else
            {
                // Here we go!
                Task.WhenAll(jobs.Select(e =>
                {
                    logger?.LogInformation("Ready to schedule job \"{0}\".", e.JobDetail.Key.Name);
                    return scheduler.ScheduleJob(e.JobDetail, e.Trigger);
                })).Wait();
                scheduler.Start().Wait();
            }

            return app;
        }
    }
}
