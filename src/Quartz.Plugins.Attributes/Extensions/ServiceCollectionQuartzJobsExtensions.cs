using System;
using System.Collections.Specialized;
using Microsoft.Extensions.DependencyInjection;

namespace Quartz
{
    using Plugins.Attributes;

    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionQuartzJobsExtensions
    {
        /// <summary>
        /// Adds Quartz.NET services (SchedulerFactory, Triggers, Jobs, etc.) to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services)
        {
            return services.AddQuartzJobs((q) => { });
        }

        /// <summary>
        /// Adds Quartz.NET services (SchedulerFactory, Triggers, Jobs, etc.) to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services, Action<IServiceCollectionQuartzConfigurator> configure)
        {
            return services.AddQuartzJobs(new NameValueCollection(), configure);
        }

        /// <summary>
        /// Adds Quartz.NET services (SchedulerFactory, Triggers, Jobs, etc.) to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="properties"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddQuartzJobs(this IServiceCollection services, NameValueCollection properties, Action<IServiceCollectionQuartzConfigurator> configure)
        {
            QuartzJobTypeContainer container = new QuartzJobTypeContainer();
            services.AddSingleton(container);

            foreach (Type type in ReflectionHelper.GetAssembliesTypes())
            {
                if (ReflectionHelper.IsQuartzJobClass(type) && Attribute.IsDefined(type, ReflectionHelper.TypeOfQuartzJobAttribute))
                {
                    services.AddTransient(type);
                    container.Add(type);
                }
            }

            return services.AddQuartz(properties, (q) =>
            {
                q.UseInMemoryStore();

                q.UseMicrosoftDependencyInjectionJobFactory(options =>
                {
                    options.AllowDefaultConstructor = true;
                });

                q.UseSimpleTypeLoader();

                q.UseTimeZoneConverter();

                configure?.Invoke(q);
            });
        }
    }
}
