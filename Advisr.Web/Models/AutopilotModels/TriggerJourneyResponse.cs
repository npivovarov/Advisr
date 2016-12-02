using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Advisr.Web.Models.AutopilotModels
{
    public class TriggerJourneyResponse
    {
        [JsonProperty(PropertyName = "bislr_identifier")]
        public string BISLRIdentifier { get; set; }

        [JsonProperty(PropertyName = "trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty(PropertyName = "contact_id_or_email")]
        public string ContactIdOrEmail { get; set; }

        [JsonProperty(PropertyName = "rqType")]
        public string RqType { get; set; }

        [JsonProperty(PropertyName = "terminated")]
        public bool Terminated { get; set; }

        [JsonProperty(PropertyName = "taskCounter")]
        public int TaskCounter { get; set; }

        [JsonProperty(PropertyName = "chunkUuid")]
        public string ChunkUuid { get; set; }

        [JsonProperty(PropertyName = "previousSteps")]
        public List<object> PreviousSteps { get; set; }

        [JsonProperty(PropertyName = "stepUuid")]
        public string StepUuid { get; set; }

        [JsonProperty(PropertyName = "apiContact")]
        public ApiContact ApiContact { get; set; }
    }

    public class ApiContact
    {
        [JsonProperty(PropertyName = "_id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "_rev")]
        public string Rev { get; set; }

        public string Email { get; set; }

        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty(PropertyName = "api_originated")]
        public bool ApiOriginated { get; set; }

        [JsonProperty(PropertyName = "custom_fields")]
        public IList<CustomField> CustomFields { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string LeadSource { get; set; }

        [JsonProperty(PropertyName = "updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty(PropertyName = "lists")]
        public List<string> Lists { get; set; }

        [JsonProperty(PropertyName = "first_visit_at")]
        public DateTime FirstVisitAt { get; set; }
        public string Name { get; set; }
    }
}