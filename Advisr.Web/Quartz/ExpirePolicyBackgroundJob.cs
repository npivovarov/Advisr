using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Providers;
using Common.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Advisr.Web.Quartz
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpirePolicyBackgroundJobProvider
    {
        private const string messageNdays =
            "<B>TAKE ACTION NOW</B>" +
            "<br>Your current insurance - <b>{policyName} - {insurerName}</b> - expire {expireDays}. To ensure you remain insured, please<b> TAKE ACTION NOW</b> " +
            "Renewing your <b>{policyType}</b> is easy and fast with Advisr. " +
            "<br>To <b>renew your policy with {insurerName}</b>, your currentprovider, click <b>Renew Policy</b>. " +
            "Or if you are interested<b> to receive quotes</b> from other providers, click <b>Get Quote</b>. " +
            "<br><br>Want more support? Ask Advisr. ";

        private const string messageToday =
            "<B>TAKE ACTION TODAY - YOUR INSURANCE IS EXPIRING</B>" +
            "<br>Your current insurance - <b>{policyName} - {insurerName}</b> - expire {expireDays}. To ensure you remain insured, please<b> TAKE ACTION NOW</b> " +
            "Renewing your <b>{policyType}</b> is easy and fast with Advisr. " +
            "<br>To <b>renew your policy with {insurerName}</b>, your currentprovider, click <b>Renew Policy</b>. " +
            "Or if you are interested<b> to receive quotes</b> from other providers, click <b>Get Quote</b>. " +
            "<br><br>Want more support? Ask Advisr.";
        
        private const string messageExpired =
            "<B>YOUR INSURANCE HAS EXPIRED</B>" +
            "<br>Your current insurance - <b>{policyName} - {insurerName}</b> - has expired. " +
            "To ensure you remain insured, you’ll need to<b> Get Quotes</b> " +
            "Buying your <b>{policyType}</b> is easy and fast with Advisr. " +
            "To <b>receive quotes</b> for {policyType}, click <b>Get Quote</b>. Alternatively, if you’ve bought elsewhere you can still use Advisr to helpmanage this insurance." +
            "<br>To do so, simply upload your new policy documents. " +
            "<br><br>Want more support? Ask Advisr.";



        private const string JobBackgroundRunExpirePolicyJobName = "JobBackgroundExpirePolicyJob";
        private const string TriggerBackgroundRunExpirePolicyJobName = "TriggerBackgroundExpirePolicyJob";

        private static ExpirePolicyBackgroundJobProvider instance;

        private static object lockPoint = new object();

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ExpirePolicyBackgroundJobProvider()
        { }

        public static ExpirePolicyBackgroundJobProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockPoint)
                    {
                        if (instance == null)
                        {
                            instance = new ExpirePolicyBackgroundJobProvider();
                        }
                    }
                }

                return instance;
            }
        }

        public void StartOrRestartSchedulerTrigger()
        {
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            if (scheduler.IsStarted == false)
            {
                scheduler.Start();
            }

            var jobIsRunning = scheduler.CheckExists(new JobKey(JobBackgroundRunExpirePolicyJobName));
            if (jobIsRunning)
            {
                scheduler.UnscheduleJob(new TriggerKey(TriggerBackgroundRunExpirePolicyJobName));
            }

            IJobDetail job = JobBuilder.Create<ExpirePolicyBackgroundJob>()
                                       .WithIdentity(new JobKey(JobBackgroundRunExpirePolicyJobName))
                                       .Build();

            ///CronExpression cexp = new CronExpression("0 30 12 1/1 * ? *"); // every day in 12:30
            CronExpression cexp = new CronExpression("0 0/30 * 1/1 * ? *"); // every 30 minutes (just for test)

            CronScheduleBuilder cronSchedule1 = CronScheduleBuilder.CronSchedule(cexp);

            ITrigger trigger = TriggerBuilder.Create()
                            .WithIdentity(TriggerBackgroundRunExpirePolicyJobName)
                            .StartNow()
                            .WithSchedule(cronSchedule1)
                            .Build();

            scheduler.ScheduleJob(job, trigger);
        }

        internal async Task Run()
        {
            log.Info("Start Expire Policy Background Job");

            try
            {
                int sentNotifications = 0;
                List<Tuple<string, string, string, string, bool>> autopilotUpdateContacts = new List<Tuple<string, string, string, string, bool>>();

                using (IUnitOfWork unitOfWork = UnitOfWork.Create())
                {
                    var maxExpDate = DateTime.Now.Date.AddDays(30);
                    var currentDate = DateTime.Now.Date;

                    var policies = unitOfWork.PolicyRepository.GetAll().Where(a => a.Status == PolicyStatus.Confirmed && a.EndDate < maxExpDate)
                        .Select(a => new
                        {
                            id = a.Id,
                            expireDate = a.EndDate,
                            insurerName = a.Insurer.Name,
                            title = a.Title,
                            subTitle = a.SubTitle,
                            userId = a.CreatedById,
                            userAutopilotContactId = a.CreatedBy.AutopilotContactId,
                            autopilotTrack = a.CreatedBy.AutopilotTrack,
                            policyType = a.PolicyType.GroupName + " " + a.PolicyType.PolicyTypeName
                        }).ToList();

                    foreach (var policy in policies)
                    {
                        NotificationTargetType notificationTargetType = NotificationTargetType.None;

                        var expDays = (policy.expireDate.Value.Date - currentDate).Days;

                        string expireDays = null;
                        string message = null;
                        string title = null;
                        string subjectFirst = string.Format("{0} {1}", policy.title, policy.subTitle);
                        string subjectSecond = string.Format("Insurance expires {0}", expireDays);
                        
                        if (expDays < 0)
                        {
                            notificationTargetType = NotificationTargetType.ExpirePolicyExpired;
                            expireDays = "today";
                            message = messageExpired;
                            title = string.Format("Your {0} {1} {2} Insurance policy has expired", policy.insurerName, policy.title, policy.subTitle);
                            subjectSecond = "Your insurance has expired";
                        }
                        else if(expDays == 0)
                        {
                            notificationTargetType = NotificationTargetType.ExpirePolicyToday;
                            expireDays = "today";
                            message = messageToday;
                            title = string.Format("Your {0} {1} {2} Insurance policy will need to be renewed today", policy.insurerName, policy.title, policy.subTitle);
                        }
                        else if (expDays <= 10)
                        {
                            notificationTargetType = NotificationTargetType.ExpirePolicy10days;
                            expireDays = expDays == 1 ? "in 1 day" : string.Format("in {0} days", expDays);
                            message = messageNdays;
                            title = string.Format("Your {0} {1} {2} Insurance policy will need to be renewed before {3:d}", policy.insurerName, policy.title, policy.subTitle, policy.expireDate);
                        }
                        else if (expDays <= 30)
                        {
                            notificationTargetType = NotificationTargetType.ExpirePolicy30days;
                            expireDays = string.Format("in {0} days", expDays);
                            message = messageNdays;
                            title = string.Format("Your {0} {1} {2} Insurance policy will need to be renewed before {3:d}", policy.insurerName, policy.title, policy.subTitle, policy.expireDate);
                        }
                        else
                        {
                            continue;
                        }

                        var notificationWasSent = unitOfWork.UserNotificationRepository.GetAll().Any(a => a.TargetObjectId != null && a.TargetObjectId.Value == policy.id && a.TargetObjectType == notificationTargetType);

                        if (notificationWasSent == false)
                        {
                           
                            message = message.Replace("{insurerName}", policy.insurerName);
                            message = message.Replace("{policyName}", policy.title + " " + policy.subTitle);
                            message = message.Replace("{expireDays}", expireDays);
                            message = message.Replace("{policyType}", policy.policyType);

                            await NotificationProvider.Instance.SendNotificationExpirePolicy(unitOfWork,
                                   title,
                                   subjectFirst,
                                   subjectSecond,
                                   message,
                                   policy.id,
                                   notificationTargetType,
                                   policy.userId
                                   );

                            string journeyName;
                            switch (notificationTargetType)
                            {
                                case NotificationTargetType.ExpirePolicyToday:
                                    journeyName = ConfigurationManager.AppSettings["PolicyExpiryToday"];
                                    break;
                                case NotificationTargetType.ExpirePolicy10days:
                                    journeyName = ConfigurationManager.AppSettings["PolicyExpire10Days"];
                                    break;
                                case NotificationTargetType.ExpirePolicy30days:
                                    journeyName = ConfigurationManager.AppSettings["PolicyExpire30Days"];
                                    break;
                                case NotificationTargetType.ExpirePolicyExpired:
                                    journeyName = ConfigurationManager.AppSettings["PolicyExpired"];
                                    break;
                                case NotificationTargetType.Profile:
                                case NotificationTargetType.None:
                                default:
                                    throw new NotImplementedException();
                            }

                            
                            Tuple<string, string, string, string, bool> autopilotContactModel = new Tuple<string, string, string, string, bool>(policy.userId, policy.userAutopilotContactId, "TriggerJourney", journeyName, policy.autopilotTrack);

                            autopilotUpdateContacts.Add(autopilotContactModel);
                            

                            //TODO: Kate:  Send email to customer 
                            //TODO: Kate:  Update autopilot 
                            //policy.userAutopilotContactId

                            sentNotifications++;
                        }
                    }

                    if (autopilotUpdateContacts.Any())
                    {
                        SendAutopilotEvents(autopilotUpdateContacts);
                    }
                }

                log.InfoFormat("End Expire Policy Background Job: Sent Notifications = {0}", sentNotifications);
            }
            catch (Exception e)
            {
                log.Fatal("End Expire Policy Background Job with error:", e);
            }
        }

        private void SendAutopilotEvents(List<Tuple<string, string, string, string, bool>> autopilotUpdateContacts)
        {
            ISchedulerFactory factory = new StdSchedulerFactory();
            IScheduler scheduler = factory.GetScheduler();
            JobDataMap dataMap = new JobDataMap();
            dataMap["autopilotUpdateContacts"] = autopilotUpdateContacts;
            dataMap["OperationType"] = "TriggerJourney";

            var job = JobBuilder.Create<UpdateContactsJob>()
                .WithIdentity("UpdateContactsJob").UsingJobData(dataMap)
                .Build();

            var jobKey = new JobKey("UpdateContactsJob");

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("UpdateContactsJobTrigger")
                .StartAt(DateTime.Now)
                .ForJob(jobKey)
                .Build();

            if (!scheduler.CheckExists(jobKey))
            {
                scheduler.ScheduleJob(job, trigger);
            }

            scheduler.Start();
        }
    }

    public class ExpirePolicyBackgroundJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            ExpirePolicyBackgroundJobProvider.Instance.Run().GetAwaiter().GetResult();
        }
    }
}