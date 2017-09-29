using Quartz;
using Quartz.Impl;

namespace Shop.TimerTask.Jobs.Wallets
{
    public class IncentiveJobScheduler
    {
        public static void Start()
        {

            IJobDetail job = JobBuilder.Create<IncentiveJob>()
                                  .WithIdentity("IncentiveJob")
                                  .Build();

            ITrigger trigger = TriggerBuilder.Create()
                                            .WithDailyTimeIntervalSchedule
                                              (s =>
                                                 s.WithIntervalInSeconds(30)
                                                .OnEveryDay()
                                              )
                                             .ForJob(job)
                                             .WithIdentity("IncentiveJobTrigger")
                                             .StartNow()
                                             .WithCronSchedule("0 5 23 ? * MON,TUE,WED,THU,SUN") //周1-周4，周日
                                             .Build();

            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sc = sf.GetScheduler();
            sc.ScheduleJob(job, trigger);
            sc.Start();
        }
    }
}
