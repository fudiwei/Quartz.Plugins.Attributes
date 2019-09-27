using System;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    using Quartz.Plugins.Attributes;

    /// <summary>
    /// 
    /// </summary>
    public static class ServiceCollectionQuartzExtensions
    {
        /// <summary>
        /// Adds Quartz.NET services (SchedulerFactory, Triggers, Jobs, etc.) to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddQuartz(this IServiceCollection services)
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

            return Quartz.DependencyInjection.Microsoft.Extensions.ServiceCollectionExtensions.AddQuartz(services);
        }
    }
}
