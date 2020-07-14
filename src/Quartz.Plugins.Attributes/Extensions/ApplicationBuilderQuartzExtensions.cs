using System;
using Microsoft.AspNetCore.Builder;

namespace Quartz
{
    /// <summary>
    /// 
    /// </summary>
    public static class ApplicationBuilderQuartzExtensions
    {
        /// <summary>
        /// Start to schedule all Quartz.NET jobs.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseQuartz(this IApplicationBuilder app)
        {
            app.ApplicationServices.UseQuartz();

            return app;
        }
    }
}
