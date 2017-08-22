using Quartz;
using Quartz.Impl;
using System.Diagnostics;

namespace Shop.TimerTask.Jobs.Orders
{
    /// <summary>
    /// 计划任务调度
    /// </summary>
    public class OrderJobScheduler
    {

        public static void Start()
        {
            Debug.WriteLine("付款过期订单任务启动");

            IJobDetail job = JobBuilder.Create<ExpiredOrderJob>()
                                  .WithIdentity("job1")
                                  .Build();

            ITrigger trigger = TriggerBuilder.Create()
                                            .WithDailyTimeIntervalSchedule
                                              (s =>
                                                 s.WithIntervalInSeconds(30)
                                                .OnEveryDay()
                                              )
                                             .ForJob(job)
                                             .WithIdentity("trigger1")
                                             .StartNow()
                                             .WithCronSchedule("0 0/1 * * * ?")//每一分钟执行一次
                                             .Build();

            ISchedulerFactory sf = new StdSchedulerFactory();
            IScheduler sc = sf.GetScheduler();
            sc.ScheduleJob(job, trigger);
            sc.Start();
        }
    }
}