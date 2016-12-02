using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Advisr.Web.Controllers.AutopilotTestService
{
    public class AutopilotService
    {
        Uri baseAddress = new Uri("https://api2.autopilothq.com/");

        public async Task<string> AddContact(string email)
        {

            string autopilotJSON = "{ \"contact\":{  \"FirstName\": \"UNKNOWN\",  \"LastName\": \"UNKNOWN\",  \"Email\": \"" + email + "\",  \"_autopilot_list\": \"contactlist_445753FE-1CC5-4466-B05F-0919FEA79CD5\",  \"LeadSource\": \"AdvisrApp\",  \"custom\": { \"string--Test--Field\": \"This is a test\" } }}";

            try
            {
                using (var httpClient = new HttpClient { BaseAddress = baseAddress })
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("autopilotapikey", "d82aa6c275bb46459886124aa4818cc0");
                    using (var content = new StringContent(autopilotJSON, System.Text.Encoding.Default, "application/json"))
                    {
                        using (var response = httpClient.PostAsync("/v1/contact", content).Result)
                        {
                            string responseData = response.Content.ReadAsStringAsync().Result;
                            return responseData;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }
        }

        public async Task<string> GetAllContacts()
        {
            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("autopilotapikey", "d82aa6c275bb46459886124aa4818cc0");
                using (var response = httpClient.GetAsync("v1/contacts").Result)
                {
                    string responseData = response.Content.ReadAsStringAsync().Result;
                    return responseData;
                }
            }
        }
    }
}