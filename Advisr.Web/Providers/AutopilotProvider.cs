using Advisr.DataLayer;
using Advisr.Domain.DbModels;
using Advisr.Web.Models;
using Advisr.Web.Models.AutopilotModels;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace Advisr.Web.Providers
{
    /// <summary>
    /// Works with autopilot api client.
    /// </summary>
    public class AutopilotProvider
    {
        private static readonly ILog log = LogManager.GetLogger("AutopilotLogger");

        AutopilotAPIClient autopilotAPIClient;

        private AutopilotProvider()
        {
            if (autopilotAPIClient == null)
            {
                autopilotAPIClient = AutopilotAPIClient.Create();
            }
        }

        private T ConvertJSONtoObject<T>(string response)
        {
            return JsonConvert.DeserializeObject<T>(response);
        }

        /// <summary>
        /// Creates an instance.
        /// </summary>
        /// <returns></returns>
        public static AutopilotProvider Create()
        {
            return new AutopilotProvider();
        }

        /// <summary>
        /// Starts on "Edit profile". Updates contact details on autopilot.
        /// </summary>
        /// <param name="autopilot_contact_id">contact id</param>
        /// <param name="model">model of customer</param>
        /// <param name="userId">id of user</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateContactDetails(string autopilot_contact_id, CustomerModel model, string userId)
        {
            var getContactresult = await autopilotAPIClient.GetContact(autopilot_contact_id);

            string autopilotcontact_id;

            if (!getContactresult.HasError)
            {
                var contactToUpdate = new Contact()
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = getContactresult.Result.Email,
                    LeadSource = getContactresult.Result.LeadSource,
                };

                contactToUpdate.ListToAdd = ConfigurationManager.AppSettings["ProfileUpdated"];
                contactToUpdate.ListsToRemoveFrom = new List<AutopilotList>()
                        {
                             new AutopilotList() { Title = ConfigurationManager.AppSettings["RegistrationList"] },
                             new AutopilotList() { Title = ConfigurationManager.AppSettings["Asleep"] }
                        };

                var customFields = getContactresult.Result.CustomFields;

                contactToUpdate.Custom = new Custom();
                contactToUpdate.Custom.FirstPolicyLoad = customFields.FirstOrDefault(f => f.Kind == "FirstPolicyLoad") != null ? (bool)customFields.FirstOrDefault(f => f.Kind == "FirstPolicyLoad").Value : false;
                contactToUpdate.Custom.PolicyLoadInProgress = customFields.FirstOrDefault(f => f.Kind == "PolicyLoadInProgress") != null ? (bool)customFields.FirstOrDefault(f => f.Kind == "PolicyLoadInProgress").Value : false;
                contactToUpdate.Custom.LastAppLogin = customFields.FirstOrDefault(f => f.Kind == "LastAppLogin") != null ? (DateTime)customFields.FirstOrDefault(f => f.Kind == "LastAppLogin").Value : default(DateTime);
                contactToUpdate.Custom.AppLoginCount = 0;
                contactToUpdate.Custom.PolicyCount = 0;
                var field = getContactresult.Result.CustomFields.FirstOrDefault(f => f.Kind == "PolicyCount");
                if (field != null)
                {
                    contactToUpdate.Custom.PolicyCount = int.Parse(field.Value.ToString());
                }

                field = getContactresult.Result.CustomFields.FirstOrDefault(f => f.Kind == "AppLoginCount");

                if (field != null)
                {
                    contactToUpdate.Custom.AppLoginCount = int.Parse(field.Value.ToString());
                }

                AutopilotResponse<string> deleteResult = null;

                var lists = await autopilotAPIClient.GetAllLists();
                if (!lists.HasError)
                {
                    foreach (var list in contactToUpdate.ListsToRemoveFrom)
                    {
                        var originList = lists.Result.Lists.FirstOrDefault(l => l.Title == list.Title);
                        if (originList != null)
                        {
                            if (!string.IsNullOrEmpty(getContactresult.Result.Lists.FirstOrDefault(cl => cl == originList.ListId)))
                            {
                                deleteResult = await autopilotAPIClient.RemoveUserFromList(originList.ListId, contactToUpdate.Email);
                            }
                        }
                    }

                    var listToAdd = lists.Result.Lists.FirstOrDefault(l => l.Title == contactToUpdate.ListToAdd);

                    if (listToAdd != null)
                    {
                        if (!string.IsNullOrEmpty(getContactresult.Result.Lists.FirstOrDefault(cl => cl == listToAdd.ListId)))
                        {
                            contactToUpdate.ListToAdd = "";
                        }
                    }
                }
                else
                {
                    using (var unitOfWork = UnitOfWork.Create())
                    {
                        var autopilotError = new AutopilotErrorBuffer();
                        autopilotError.AutopilotContactId = autopilot_contact_id;
                        autopilotError.RequestData = JsonConvert.SerializeObject(model);
                        autopilotError.OperationType = "Edit";
                        autopilotError.UserId = userId;

                        unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                        await unitOfWork.SaveAsync();
                    }

                    log.Error("Method:" + MethodBase.GetCurrentMethod().DeclaringType + " .Unable to get lists.");
                }

                if (deleteResult != null && deleteResult.HasError)
                {
                    using (var unitOfWork = UnitOfWork.Create())
                    {
                        var autopilotError = new AutopilotErrorBuffer();
                        autopilotError.AutopilotContactId = autopilot_contact_id;
                        autopilotError.RequestData = JsonConvert.SerializeObject(model);
                        autopilotError.OperationType = "Edit";
                        autopilotError.UserId = userId;

                        unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                        await unitOfWork.SaveAsync();
                    }

                    log.Error("Method:" + MethodBase.GetCurrentMethod().DeclaringType + " .Could not delete contact" + autopilot_contact_id + "from list. Method:" + MethodBase.GetCurrentMethod().DeclaringType);
                    return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                }

                if (getContactresult.Result.Email != model.Email)
                {
                    contactToUpdate.NewEmail = model.Email;
                }

                var updateResult = await autopilotAPIClient.AddOrUpdateContact(new AutopilotContactToAdd() { Contact = contactToUpdate });

                if (!updateResult.HasError)
                {
                    autopilotcontact_id = updateResult.Result.ContactId;
                    log.Info("Method:" + MethodBase.GetCurrentMethod().DeclaringType + " .Contact " + autopilot_contact_id + " info has been updated.");
                    return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                }
                else
                {
                    log.Error("Method:" + MethodBase.GetCurrentMethod().DeclaringType + " .Unable to update contact " + autopilot_contact_id + ". Error:" + updateResult.Error.Error + updateResult.Error.Message);
                    using (var unitOfWork = UnitOfWork.Create())
                    {
                        var autopilotError = new AutopilotErrorBuffer();
                        autopilotError.AutopilotContactId = autopilot_contact_id;
                        autopilotError.RequestData = JsonConvert.SerializeObject(model);
                        autopilotError.OperationType = "Edit";
                        autopilotError.UserId = userId;

                        unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                        await unitOfWork.SaveAsync();
                    }
                    return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
                }
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var autopilotError = new AutopilotErrorBuffer();
                autopilotError.AutopilotContactId = autopilot_contact_id;
                autopilotError.RequestData = JsonConvert.SerializeObject(model);
                autopilotError.OperationType = "Edit";
                autopilotError.UserId = userId;

                unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                await unitOfWork.SaveAsync();
            }

            log.Error("Method:" + MethodBase.GetCurrentMethod().DeclaringType + " .Unable to get contact " + autopilot_contact_id + ". Error:" + getContactresult.Error.Error + getContactresult.Error.Message);
            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Starts on "Login" and "External login".Updates contact's last login date and login count.
        /// </summary>
        /// <param name="contact_id">contact id</param>
        /// <param name="userId">id of user</param>
        /// <returns></returns>
        public async Task<string> UpdateAutopilotContactLogin(string contact_id, string userId)
        {
            string autopilotcontact_id = "";
            var getContactResult = await autopilotAPIClient.GetContact(contact_id);
            if (!getContactResult.HasError)
            {
                var customFields = getContactResult.Result.CustomFields;
                var field = customFields.FirstOrDefault(f => f.Kind == "AppLoginCount");
                int loginCount = 0;

                if (field != null)
                {
                    loginCount = int.Parse(field.Value.ToString());
                }

                var contact = new Contact()
                {
                    FirstName = getContactResult.Result.FirstName,
                    LastName = getContactResult.Result.LastName,
                    LeadSource = getContactResult.Result.LeadSource,
                    Email = getContactResult.Result.Email,
                    Custom = new Custom()
                    {
                        FirstPolicyLoad = customFields.FirstOrDefault(f => f.Kind == "FirstPolicyLoad") == null ? false : (bool)customFields.FirstOrDefault(f => f.Kind == "FirstPolicyLoad").Value,
                        PolicyCount = customFields.FirstOrDefault(f => f.Kind == "PolicyCount") == null ? 0 : int.Parse(customFields.FirstOrDefault(f => f.Kind == "PolicyCount").Value.ToString()),
                        PolicyLoadInProgress = customFields.FirstOrDefault(f => f.Kind == "PolicyLoadInProgress") == null ? false : (bool)customFields.FirstOrDefault(f => f.Kind == "PolicyLoadInProgress").Value,
                        LastAppLogin = DateTime.Now,
                        AppLoginCount = loginCount + 1,
                    }
                };

                var updateLoginInfoResult = await autopilotAPIClient.AddOrUpdateContact(new AutopilotContactToAdd() { Contact = contact });

                if (!updateLoginInfoResult.HasError)
                {
                    autopilotcontact_id = updateLoginInfoResult.Result.ContactId;
                    log.Info("Method:" + MethodBase.GetCurrentMethod().DeclaringType + " .Contact " + contact_id + " login info has been updated.");
                    return autopilotcontact_id;
                }
                else
                {
                    using (var unitOfWork = UnitOfWork.Create())
                    {
                        var autopilotError = new AutopilotErrorBuffer();
                        autopilotError.AutopilotContactId = contact_id;
                        autopilotError.OperationType = "Login";
                        autopilotError.UserId = userId;

                        unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                        await unitOfWork.SaveAsync();
                    }

                    log.Error("Method:" + MethodBase.GetCurrentMethod().DeclaringType + " .Unable to update contact " + contact_id + " . Error: " + updateLoginInfoResult.Error.Error + updateLoginInfoResult.Error.Message);
                }
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var autopilotError = new AutopilotErrorBuffer();
                autopilotError.AutopilotContactId = contact_id;
                autopilotError.OperationType = "Login";
                autopilotError.UserId = userId;

                unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                await unitOfWork.SaveAsync();
            }

            log.Error("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Unable to get contact " + contact_id + " . Error: " + getContactResult.Error.Error + getContactResult.Error.Message);
            return string.Empty;
        }

        /// <summary>
        /// Starts on registration or external registration(G+,FB). Creates autopilot contact and associates it with created user.
        /// </summary>
        /// <param name="model">model of registration</param>
        /// <param name="userId">id of created user</param>
        /// <returns></returns>
        public async Task<string> CreateAutopilotContact(RegisterGeneralModel model, string userId)
        {
            var autopilotContact = new Contact();
            autopilotContact.FirstName = model.FirstName;
            autopilotContact.LastName = model.LastName;
            autopilotContact.Email = model.Email;
            autopilotContact.LeadSource = "AdvisrApp";
            autopilotContact.ListToAdd = ConfigurationManager.AppSettings["RegistrationList"];
            autopilotContact.Custom = new Custom();
            autopilotContact.Custom.PolicyLoadInProgress = false;
            autopilotContact.Custom.LastAppLogin = DateTime.Now;
            autopilotContact.Custom.PolicyCount = 0;
            autopilotContact.Custom.AppLoginCount = 0;
            autopilotContact.Custom.FirstPolicyLoad = false;

            autopilotAPIClient = AutopilotAPIClient.Create();

            string autopilotcontact_id = "";
            var addContactResult = await autopilotAPIClient.AddOrUpdateContact(new AutopilotContactToAdd() { Contact = autopilotContact });

            using (var unitOfWork = UnitOfWork.Create())
            {
                if (!addContactResult.HasError)
                {
                    var user = unitOfWork.UserRepository.GetAll().FirstOrDefault(u => u.Id == userId);
                    if (user != null)
                    {
                        autopilotcontact_id = addContactResult.Result.ContactId;
                        user.AutopilotContactId = autopilotcontact_id;

                        var getContactResult = await autopilotAPIClient.GetContact(autopilotcontact_id);

                        if (!getContactResult.HasError)
                        {
                            user.AutopilotData = JsonConvert.SerializeObject(getContactResult);
                        }

                        unitOfWork.UserRepository.Edit(user);
                        await unitOfWork.SaveAsync();

                        log.Info("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Contact " + autopilotcontact_id + " has been created.");

                        return autopilotcontact_id;
                    }
                }
                else
                {

                    var autopilotError = new AutopilotErrorBuffer();
                    autopilotError.AutopilotContactId = "";
                    autopilotError.OperationType = "Registration";
                    autopilotError.RequestData = JsonConvert.SerializeObject(model);
                    autopilotError.UserId = userId;

                    unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                    await unitOfWork.SaveAsync();

                    log.Error("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Contact's " + autopilotcontact_id + " creation has been failed.");
                    autopilotcontact_id = "";
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Starts on "Create policy". Updates contact's fields about policy loading progress.
        /// </summary>
        /// <param name="autopilot_contact_id">contact id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> UpdateContactPolicyLoad(string autopilot_contact_id, string userId)
        {
            var getContactResult = await autopilotAPIClient.GetContact(autopilot_contact_id);
            if (!getContactResult.HasError)
            {
                var customFields = getContactResult.Result.CustomFields;
                var field = customFields.FirstOrDefault(f => f.Kind == "AppLoginCount");
                int loginCount = 0;

                if (field != null)
                {
                    loginCount = int.Parse(field.Value.ToString());
                }

                var contact = new Contact()
                {
                    FirstName = getContactResult.Result.FirstName,
                    LastName = getContactResult.Result.LastName,
                    LeadSource = getContactResult.Result.LeadSource,
                    Email = getContactResult.Result.Email,
                    Custom = new Custom()
                    {
                        FirstPolicyLoad = customFields.FirstOrDefault(f => f.Kind == "FirstPolicyLoad") == null ? false : (bool)customFields.FirstOrDefault(f => f.Kind == "FirstPolicyLoad").Value,
                        PolicyCount = customFields.FirstOrDefault(f => f.Kind == "PolicyCount") == null ? 0 : int.Parse(customFields.FirstOrDefault(f => f.Kind == "PolicyCount").Value.ToString()),
                        PolicyLoadInProgress = true,
                        LastAppLogin = customFields.FirstOrDefault(f => f.Kind == "LastAppLogin") == null ? DateTime.Now : (DateTime)customFields.FirstOrDefault(f => f.Kind == "LastAppLogin").Value,
                        AppLoginCount = loginCount,
                    }
                };

                using (var unitOfWork = UnitOfWork.Create())
                {
                    var usersPolicies = unitOfWork.PolicyRepository.GetAll().Where(p => p.CreatedById == userId);

                    if (usersPolicies != null)
                    {
                        if (usersPolicies.Count() == 1)
                        {
                            contact.Custom.FirstPolicyLoad = true;
                        }
                        else
                        {
                            contact.Custom.FirstPolicyLoad = false;
                        }

                        contact.Custom.PolicyCount = usersPolicies.Count();

                        var updatePolicyInfoResult = await autopilotAPIClient.AddOrUpdateContact(new AutopilotContactToAdd() { Contact = contact });

                        if (!updatePolicyInfoResult.HasError)
                        {
                            autopilot_contact_id = updatePolicyInfoResult.Result.ContactId;
                            log.Info("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Contact's" + autopilot_contact_id + "policy load info was updated.");
                            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                        }
                        else
                        {
                            var autopilotError = new AutopilotErrorBuffer();
                            autopilotError.AutopilotContactId = autopilot_contact_id;
                            autopilotError.OperationType = "PolicyLoad";
                            autopilotError.UserId = userId;

                            unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                            await unitOfWork.SaveAsync();

                            log.Error("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Contact's " + autopilot_contact_id + " policy load update has been failed. Error:" + getContactResult.Error.Error + ' ' + getContactResult.Error.Message);
                        }
                    }
                }
                using (var unitOfWork = UnitOfWork.Create())
                {
                    var autopilotError = new AutopilotErrorBuffer();
                    autopilotError.AutopilotContactId = autopilot_contact_id;
                    autopilotError.OperationType = "PolicyLoad";
                    autopilotError.UserId = userId;

                    unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                    await unitOfWork.SaveAsync();
                }

                log.Error("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Contact's " + autopilot_contact_id + " policy load update has been failed." + " . Error:" + getContactResult.Error.Error + ' ' + getContactResult.Error.Message);
                return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var autopilotError = new AutopilotErrorBuffer();
                autopilotError.AutopilotContactId = autopilot_contact_id;
                autopilotError.OperationType = "PolicyLoad";
                autopilotError.UserId = userId;

                unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                await unitOfWork.SaveAsync();
            }

            log.Error("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Unable to get contact" + autopilot_contact_id + " . Error:" + getContactResult.Error.Error + ' ' + getContactResult.Error.Message);
            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Starts on admin's confirmation of policy. Adds contact to journey.
        /// </summary>
        /// <param name="journeyName">name of the journey</param>
        /// <param name="autopilot_contact_id">contact id</param>
        /// <param name="userId">id of user</param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> TriggerUserToJourney(string journeyName, string autopilot_contact_id, string userId)
        {
            var triggerJourneyResult = await autopilotAPIClient.AddUserToJourney(journeyName, autopilot_contact_id);

            if (!triggerJourneyResult.HasError)
            {
                log.Info("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Contact " + autopilot_contact_id + " was successfully added to journey" + journeyName);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }

            using (var unitOfWork = UnitOfWork.Create())
            {
                var autopilotError = new AutopilotErrorBuffer();
                autopilotError.AutopilotContactId = autopilot_contact_id;
                autopilotError.OperationType = "TriggerJourney";
                autopilotError.RequestData = journeyName;
                autopilotError.UserId = userId;

                unitOfWork.AutopilotErrorRepository.Insert(autopilotError);
                await unitOfWork.SaveAsync();
            }

            log.Error("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .Contact " + autopilot_contact_id + " addition was failed. Journey: " + journeyName);
            return new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Makes attempt to finish failed autopilot operations.
        /// </summary>
        public void CheckErrorBuffer()
        {
            using (var unitOfWork = UnitOfWork.Create())
            {
                var errors = unitOfWork.AutopilotErrorRepository.GetAll();
                string info;

                if (errors.Count() == 0)
                {
                    log.Info("Method: " + MethodBase.GetCurrentMethod().DeclaringType + " .No autopilot errors were found.");
                    return;
                }

                foreach (var error in errors)
                {
                    switch (error.OperationType)
                    {
                        case "Edit":
                            string autopilot_contact_id = error.AutopilotContactId;
                            CustomerModel model = ConvertJSONtoObject<CustomerModel>(error.RequestData);
                            string userId = error.UserId;
                            var result = UpdateContactDetails(autopilot_contact_id, model, userId).Result;
                            break;
                        case "Login":
                            userId = error.UserId;
                            autopilot_contact_id = error.AutopilotContactId;
                            info = UpdateAutopilotContactLogin(autopilot_contact_id, userId).Result;
                            break;
                        case "Registration":
                            userId = error.UserId;
                            RegisterGeneralModel registermodel = ConvertJSONtoObject<RegisterGeneralModel>(error.RequestData);
                            info = CreateAutopilotContact(registermodel, userId).Result;
                            break;
                        case "PolicyLoad":
                            userId = error.UserId;
                            autopilot_contact_id = error.AutopilotContactId;
                            result = UpdateContactPolicyLoad(autopilot_contact_id, userId).Result;
                            break;
                        case "TriggerJourney":
                            userId = error.UserId;
                            string journeyName = error.RequestData;
                            autopilot_contact_id = error.AutopilotContactId;
                            result = TriggerUserToJourney(journeyName, autopilot_contact_id, userId).Result;
                            break;
                        default:
                            break;
                    }

                    unitOfWork.AutopilotErrorRepository.Delete(error);
                }

                unitOfWork.Save();
            }
        }
    }
}