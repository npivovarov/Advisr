using Advisr.Web.Controllers.AutopilotTestService;
using Advisr.Web.Models.AutopilotModels;
using Advisr.Web.Models.AutopilotTest;
using Advisr.Web.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Advisr.Web.Controllers
{
    [Route("api/AutopilotTest")]
    public class AutopilotTestController : ApiController
    {
        AutopilotAPIClient autopilot = AutopilotAPIClient.Create();
        // GET: api/Contact
        [HttpGet]
        public AutopilotLists Get()
        {
            try
            {
                AutopilotLists result = autopilot.GetAllLists().Result.Result;
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // POST: api/Contact
        [HttpPost]
        [Route("api/AutopilotTest/AddContact")]
        public HttpResponseMessage AddContact(Contact contact)
        {
            if (contact.Email != null)
            {
                string res = null;
            }

            return new HttpResponseMessage(HttpStatusCode.Created);
        }

        [HttpPost]
        [Route("api/AutopilotTest/TriggerJourney")]
        public async Task<IHttpActionResult> TriggerJourney(string contactId, string JourneyName)
        {
            var result = await autopilot.AddUserToJourney(JourneyName, contactId);
            if (result.HasError)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
