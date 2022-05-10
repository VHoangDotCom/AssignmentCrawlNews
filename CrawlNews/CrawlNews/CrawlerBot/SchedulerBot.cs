using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CrawlNews.CrawlerBot
{
    public class SchedulerBot
    {

        public async Task Execute_10_sec()
        {
            // Grab the Scheduler instance from the Factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = await factory.GetScheduler();
           
            // and start it off
            await scheduler.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("job1", "group1")
                .Build();

            //Trigger execute job
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("trigger1", "group1")
              .StartNow()
              .WithSimpleSchedule(x => x
                  .WithIntervalInSeconds(10)
                  .RepeatForever())
              .Build();

            // Tell quartz to schedule the job using our trigger
            await scheduler.ScheduleJob(job, trigger);

            // some sleep to show what's happening
            await Task.Delay(TimeSpan.FromSeconds(60));

            // and last shut down the scheduler when you are ready to close your program
            await scheduler.Shutdown();
        }

        //Cac Trigger lap lich theo tuy chon
        //Trigger chay ngay bay gio, lap lai moi 10s 1 lan
        public static ITrigger Trigger_10sec(string group)
        {
            ITrigger trigger = TriggerBuilder.Create()
               .WithIdentity("trigger1", "group1")
               .StartNow()
               .WithSimpleSchedule(x => x
                   .WithIntervalInSeconds(10)
                   .RepeatForever())
               .Build();

            return trigger;
        }

        //Trigger thuc hien moi ngay vao luc 10h moi ngay
        public static ITrigger Daily_at_Hour(string job, string group)
        {
            ITrigger trigger = TriggerBuilder.Create()
              .WithIdentity("trigger3", group)
              .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(10, 00))
              .ForJob(job, group)
              .Build();
            return trigger;
        }

        //Trigger thuc hien moi thu tu hang tuan luc 10h42; timezone chinh sua
        public static ITrigger SpecificDay_Hour(DayOfWeek day, string job, string group)
        {
            ITrigger trigger = TriggerBuilder.Create()
                 .WithIdentity("trigger3", group)
                 .WithSchedule(CronScheduleBuilder
                     .WeeklyOnDayAndHourAndMinute(day, 10, 42)
                     .InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Central America Standard Time")))
                 .ForJob(job, group)
                 .Build();
            return trigger;
        }

        //Trigger thuc hien hang ngay moi phut, tu 8am - 5pm
        public static ITrigger Minutes(DayOfWeek day, string job, string group)
        {
            ITrigger trigger = TriggerBuilder.Create()
                 .WithIdentity("trigger3", group)
                 .WithCronSchedule("0 0/2 8-17 * * ?")
                 .ForJob(job, group)
                 .Build();
            return trigger;
        }

        /*       Expression Meaning
       0 0 12 * * ?	Fire at 12pm(noon) every day
       0 15 10 ? * *	Fire at 10:15am every day
       0 15 10 * * ?	Fire at 10:15am every day
       0 15 10 * * ? *	Fire at 10:15am every day
       0 15 10 * * ? 2005	Fire at 10:15am every day during the year 2005
       0 * 14 * * ?	Fire every minute starting at 2pm and ending at 2:59pm, every day
       0 0/5 14 * * ?	Fire every 5 minutes starting at 2pm and ending at 2:55pm, every day
       0 0/5 14,18 * * ?	Fire every 5 minutes starting at 2pm and ending at 2:55pm, AND fire every 5 minutes starting at 6pm and ending at 6:55pm, every day
       0 0-5 14 * * ?	Fire every minute starting at 2pm and ending at 2:05pm, every day
       0 10,44 14 ? 3 WED Fire at 2:10pm and at 2:44pm every Wednesday in the month of March.
       0 15 10 ? * MON-FRI Fire at 10:15am every Monday, Tuesday, Wednesday, Thursday and Friday
       0 15 10 15 * ?	Fire at 10:15am on the 15th day of every month
       0 15 10 L* ?	Fire at 10:15am on the last day of every month
       0 15 10 L-2 * ?	Fire at 10:15am on the 2nd-to-last last day of every month
       0 15 10 ? * 6L	Fire at 10:15am on the last Friday of every month
       0 15 10 ? * 6L	Fire at 10:15am on the last Friday of every month
       0 15 10 ? * 6L 2002-2005	Fire at 10:15am on every last friday of every month during the years 2002, 2003, 2004 and 2005
       0 15 10 ? * 6#3	Fire at 10:15am on the third Friday of every month
       0 0 12 1/5 * ?	Fire at 12pm (noon) every 5 days every month, starting on the first day of the month.
       0 11 11 11 11 ?	Fire every November 11th at 11:11am.

       */

    }

    public class HelloJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("Greetings from HelloJob!");
            //Su dung RabbitMQ crawl link tai day

        }
    }
}