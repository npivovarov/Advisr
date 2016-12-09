using Advisr.Web.Models.AutopilotModels;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Advisr.Web.Providers
{
    public class AutopilotAPIClient
    {
        private static volatile HttpClient client;
        Uri resultUri;
        private AutopilotAPIClient()
        {
            if (client == null)
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("autopilotapikey",
                    ConfigurationManager.AppSettings["autopilotapikey"]);
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["autopilotApiUrl"]);
            } 
        }

        public static AutopilotAPIClient Create()
        {
            return new AutopilotAPIClient();
        }

        private string ConvertObjecttoJSON<T>(T model)
        {
            
            string jsonString = JsonConvert.SerializeObject(model);
            return jsonString;            
        }

        private T ConvertJSONtoObject<T>(string response)
        {
             return JsonConvert.DeserializeObject<T>(response);            
        }

        /// <summary>
        /// Adds or updates contact at autopilot.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<AutopilotResponse<AutopilotContactId>> AddOrUpdateContact(AutopilotContactToAdd model)
        {
            if (!string.IsNullOrEmpty(model.Contact.ListToAdd))
            {
                var getListsResult = await GetAllLists();
                if (getListsResult.HasError)
                {
                    return new AutopilotResponse<AutopilotContactId>()
                    {
                        Result = null,
                        Error = getListsResult.Error,
                        HasError = true
                    };
                }
                
                var list = getListsResult.Result.Lists.FirstOrDefault(l => l.Title == model.Contact.ListToAdd);
                var listId = list == null ? "" : list.ListId;

                if (string.IsNullOrEmpty(listId))
                {
                    return new AutopilotResponse<AutopilotContactId>
                    {
                        Result = null,
                        Error = new AutopilotError
                        {
                            Error = "404",
                            Message = "List not found"
                        },
                        HasError = true
                    };
                }

                if (!string.IsNullOrEmpty(listId))
                {
                    model.Contact.AutopilotList = listId;
                }
            }

            string autopilotJSON = ConvertObjecttoJSON(model);

            using (var content = new StringContent(autopilotJSON, System.Text.Encoding.Default, "application/json"))
            {
                using (var response = await client.PostAsync("/v1/contact", content))
                {
                    string responseData = "";
                    if (response.StatusCode == System.Net.HttpStatusCode.OK ||
                        response.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        responseData = await response.Content.ReadAsStringAsync();
                        var contact_id = ConvertJSONtoObject<AutopilotContactId>(responseData);
                        return new AutopilotResponse<AutopilotContactId>()
                        {
                            Result = contact_id,
                            Error = null,
                            HasError = false
                        };
                    }

                    var error = await response.Content.ReadAsAsync<AutopilotError>();

                    return new AutopilotResponse<AutopilotContactId>()
                    {
                        Result = null,
                        Error = error,
                        HasError = true
                    };
                }
            }
        }

        /// <summary>
        /// Gets all contact lists.
        /// </summary>
        /// <returns></returns>
        public async Task<AutopilotResponse<AutopilotLists>> GetAllLists()
        {
            using (var response = await client.GetAsync("v1/lists"))
            {
                string responseData;

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseData = await response.Content.ReadAsStringAsync();
                    var lists = ConvertJSONtoObject<AutopilotLists>(responseData);

                    return new AutopilotResponse<AutopilotLists>()
                    {
                        Result = lists,
                        Error = null,
                        HasError = false
                    };
                }

                responseData = await response.Content.ReadAsStringAsync();
                var error = ConvertJSONtoObject<AutopilotError>(responseData);
                return new AutopilotResponse<AutopilotLists>()
                {
                    Result = null,
                    Error = error,
                    HasError = true
                };

            }
        }

        /// <summary>
        /// Gets contact by contact id.
        /// </summary>
        /// <param name="contact_id"></param>
        /// <returns></returns>
        public async Task<AutopilotResponse<AutopilotContact>> GetContact(string contact_id)
        {
            using (var response = await client.GetAsync("v1/contact/" + contact_id))
            {
                string responseData = "";
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseData = await response.Content.ReadAsStringAsync();
                    var contact = ConvertJSONtoObject<AutopilotContact>(responseData);

                    return new AutopilotResponse<AutopilotContact>()
                    {
                        Result = contact,
                        Error = null,
                        HasError = false
                    };
                }

                responseData = await response.Content.ReadAsStringAsync();
                var error = ConvertJSONtoObject<AutopilotError>(responseData);

                return new AutopilotResponse<AutopilotContact>
                {
                    Result = null,
                    Error = error,
                    HasError = true
                };
            }
        }

        /// <summary>
        /// Checks if contact is in the contact list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="idOrEmail"></param>
        /// <returns></returns>
        public async Task<AutopilotResponse<bool>> IsInTheList(string listId, string idOrEmail)
        {
            if (string.IsNullOrEmpty(listId))
            {
                return new AutopilotResponse<bool>
                {
                    Result = false,
                    Error = new AutopilotError
                    {
                        Error = "404",
                        Message = "List not found"
                    },
                    HasError = true
                };
            }
            var contact = await GetContact(idOrEmail);

            if (contact.HasError && contact.Result == null)
            {
                return new AutopilotResponse<bool>
                {
                    Result = false,
                    Error = new AutopilotError
                    {
                        Error = "404",
                        Message = "Contact not found"
                    },
                    HasError = true
                };
            }

            using (var response = await client.GetAsync("v1/list/" + listId + "/contact/" + idOrEmail))
            {
                string responseData = "";
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseData = await response.Content.ReadAsStringAsync();

                    return new AutopilotResponse<bool>()
                    {
                        Result = true,
                        Error = null,
                        HasError = false
                    };
                }

                return new AutopilotResponse<bool>()
                {
                    Result = false,
                    Error = null,
                    HasError = false
                };
            }
        }

        /// <summary>
        /// Deletes contact from the contact list.
        /// </summary>
        /// <param name="listId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<AutopilotResponse<string>> RemoveUserFromList(string listId, string email)
        {
            
            if (string.IsNullOrEmpty(listId))
            {
                return new AutopilotResponse<string>
                {
                    Result = null,
                    Error = new AutopilotError
                    {
                        Error = "404",
                        Message = "List not found"
                    },
                    HasError = true
                };
            }

            using (var response = await client.DeleteAsync("v1/list/" + listId + "/contact/" + email))
            {
                string responseData = "";
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseData = await response.Content.ReadAsStringAsync();
                    return new AutopilotResponse<string>()
                    {
                        Result = responseData,
                        Error = null,
                        HasError = false
                    };
                }

                var error = await response.Content.ReadAsAsync<AutopilotError>();

                return new AutopilotResponse<string>()
                {
                    Result = null,
                    Error = error,
                    HasError = true
                };
            }

        }

        private async Task<AutopilotResponse<AutopilotTriggers>> GetTriggers()
        {
            using (var response = await client.GetAsync("v1/triggers"))
            {
                string responseData = "";
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseData = await response.Content.ReadAsStringAsync();
                    var triggers = ConvertJSONtoObject<AutopilotTriggers>(responseData);

                    return new AutopilotResponse<AutopilotTriggers>()
                    {
                        Result = triggers,
                        Error = null,
                        HasError = false
                    };
                }

                responseData = await response.Content.ReadAsStringAsync();
                var error = ConvertJSONtoObject<AutopilotError>(responseData);

                return new AutopilotResponse<AutopilotTriggers>
                {
                    Result = null,
                    Error = error,
                    HasError = true
                };
            }
        }

        /// <summary>
        /// Adds user to specified journey.
        /// </summary>
        /// <param name="journeyName"></param>
        /// <param name="contact_id_email"></param>
        /// <returns></returns>
        public async Task<AutopilotResponse<TriggerJourneyResponse>> AddUserToJourney(string journeyName, string contact_id_email)
        {
            var result = await GetTriggers();
            if (result.HasError)
            {
                return new AutopilotResponse<TriggerJourneyResponse>()
                {
                    Result = null,
                    Error = result.Error,
                    HasError = true
                };
            }

            var trigger = result.Result.Triggers.FirstOrDefault(t => t.JourneyName == journeyName);

            if (trigger == null)
            {
                return new AutopilotResponse<TriggerJourneyResponse>()
                {
                    Result = null,
                    Error = new AutopilotError() { Error = "404", Message = "Journey has no API triggers or was not published." },
                    HasError = true,
                };
            }

            using (var response = await client.PostAsync("/v1/trigger/" + trigger.TriggerId + "/contact/" + contact_id_email, null))
            {
                string responseData;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    responseData = await response.Content.ReadAsStringAsync();
                    var triggerRes = ConvertJSONtoObject<TriggerJourneyResponse>(responseData);
                    return new AutopilotResponse<TriggerJourneyResponse>()
                    {
                        Result = triggerRes,
                        Error = null,
                        HasError = false
                    };
                }

                var error = await response.Content.ReadAsAsync<AutopilotError>();

                return new AutopilotResponse<TriggerJourneyResponse>()
                {
                    Result = null,
                    Error = error,
                    HasError = true
                };
            }
        }
    }
}