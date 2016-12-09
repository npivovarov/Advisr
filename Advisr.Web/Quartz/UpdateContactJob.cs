using Advisr.Web.Models;
using Advisr.Web.Providers;
using log4net;
using Quartz;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Advisr.Web.Quartz
{
    public class UpdateContactJob : IJob
    {
        AutopilotProvider provider;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UpdateContactJob()
        {
            provider = AutopilotProvider.Create();
        }

        public void Execute(IJobExecutionContext context)
        {
            provider.CheckErrorBuffer();

            JobDataMap datamap = context.JobDetail.JobDataMap;
            
            string operation = datamap.GetString("OperationType");
            string info;
            try
            {
                switch (operation)
                {
                    case "Edit":
                        string autopilot_contact_id = datamap.GetString("autopilotContactId");
                        CustomerModel model = (CustomerModel)datamap["model"];
                        string userId = datamap.GetString("userId");
                        var result = provider.UpdateContactDetails(autopilot_contact_id, model, userId).Result;
                        break;
                    case "Login":
                        autopilot_contact_id = datamap.GetString("autopilotContactId");
                        userId = datamap.GetString("userId");
                        info = provider.UpdateAutopilotContactLogin(autopilot_contact_id, userId).Result;
                        break;
                    case "Registration":
                        userId = datamap.GetString("userId");
                        RegisterGeneralModel registermodel = (RegisterGeneralModel)datamap["registermodel"];
                        info = provider.CreateAutopilotContact(registermodel, userId).Result;
                        break;
                    case "PolicyLoad":
                        userId = datamap.GetString("policyauthorId");
                        autopilot_contact_id = datamap.GetString("autopilotContactId");
                        result = provider.UpdateContactPolicyLoad(autopilot_contact_id, userId).Result;
                        break;
                    case "TriggerJourney":
                        string journeyName = datamap.GetString("journeyName");
                        userId = datamap.GetString("userId");
                        autopilot_contact_id = datamap.GetString("autopilotContactId");
                        result = provider.TriggerUserToJourney(journeyName, autopilot_contact_id, userId).Result;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message, ex);
            }
            
        }
    }


    public class UpdateContactsJob : IJob
    {
        AutopilotProvider provider;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public UpdateContactsJob()
        {
            provider = AutopilotProvider.Create();
        }

        public void Execute(IJobExecutionContext context)
        {
            provider.CheckErrorBuffer();

            JobDataMap datamap = context.JobDetail.JobDataMap;

            string operation = datamap.GetString("OperationType");
            try
            {
                List<Tuple<string, string, string, string, bool>> contacts = (List<Tuple<string, string, string, string, bool>>)datamap["autopilotUpdateContacts"];

                foreach (var item in contacts)
                {
                    if (item.Item5)
                    {
                        var result = provider.TriggerUserToJourney(item.Item4, item.Item2, item.Item1).Result;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message, ex);
            }

        }
    }
}