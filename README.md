# Quartz.Plugins.Attributes

A convenient way to create Quartz.NET jobs using attributes.

---

## Features

* Supports scheduling jobs using attributes instead of configuration files.
* Supports dependency injection in *IJob* class (depends on the library [Quartz.DependencyInjection.Microsoft](https://github.com/nizmow/Quartz.DependencyInjection.Microsoft)).
* Supports logger provider created by *ILoggerFactory*, instead of the default.
* Follows the lifecycle of ASP.NET Core.

---

## Usage

Here is an example:

``` CSharp
using Quartz;

[QuartzJob(Name = "Clocking", Description = "Tell the time every seconds.", CronExpression = "* * * * * ? ")]
public class ClockingJob : IJob
{
    private readonly ILogger _logger;

    public ClockingJob(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger(GetType());
    }

    async Task IJob.Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Now is " + context.FireTimeUtc);
        await Task.CompletedTask;
    }
}
```

Also, there are options for *Name*, *Group*, *Description* *CronExpression*, *Priority*, *StoreDurably* and *RequestRecovery*.

Register service in *ConfigureServices* method in `Startup.cs` file.

``` CSharp
using Microsoft.Extensions.DependencyInjection;
using Quartz;

public void ConfigureServices(IServiceCollection services)
{
    // do something
    services.AddQuartz();
}
```

Then Start Quartz scheduler in *Configure* method in `Startup.cs` file:

``` CSharp
using Microsoft.AspNetCore.Builder;
using Quartz;

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    // do something
    app.UseQuartz();
}
```