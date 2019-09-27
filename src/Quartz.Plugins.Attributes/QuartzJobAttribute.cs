using System;

namespace Quartz
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class QuartzJobAttribute : Attribute
    {
        /// <summary>
        /// The name element for the Job's JobKey.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The group element for the Job's JobKey.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// The given (human-meaningful) description of the Job.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The CRON expression that defines when the Trigger is fired.
        /// </summary>
        public string CronExpression { get; set; }

        /// <summary>
        /// The Trigger's priority. When more than one Trigger have the same fire time, the scheduler will fire the one with the highest priority first.
        /// </summary>
        public int Priority { get; set; } = 1;

        /// <summary>
        /// Whether or not the job should remain stored after it is orphaned. (default: false)
        /// </summary>
        public bool StoreDurably { get; set; }

        /// <summary>
        /// Instructs the <see cref="Quartz.IScheduler"/> whether or not the job should be re-executed if a 'recovery' or 'fail-over' situation is encountered. (default: false)
        /// </summary>
        public bool RequestRecovery { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public QuartzJobAttribute()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cron"></param>
        public QuartzJobAttribute(string cron)
        {
            CronExpression = cron;
        }
    }
}
