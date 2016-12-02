using Advisr.Web.Models.AutopilotModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Advisr.Web.Models
{
    public class Contact
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LastName { get; set; }

        public string Email { get; set; }

        [JsonProperty(PropertyName = "_NewEmail", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string NewEmail { get; set; }

        [JsonProperty(PropertyName = "_autopilot_list", DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string AutopilotList { get; set; }

        public string LeadSource { get; set; }

        [JsonProperty(PropertyName = "custom")]
        public Custom Custom { get; set; }

        [JsonIgnore]
        public string ListToAdd { get; set; }

        [JsonIgnore]
        public List<AutopilotList> ListsToRemoveFrom { get; set; }
    }

    public class Custom
    {
        [JsonProperty(PropertyName = "boolean--PolicyLoadInProgress")]
        public bool PolicyLoadInProgress { get; set; }

        [JsonProperty(PropertyName = "date--LastAppLogin")]
        public DateTime LastAppLogin { get; set; }

        [JsonProperty(PropertyName = "integer--PolicyCount")]
        public int PolicyCount { get; set; }

        [JsonProperty(PropertyName = "boolean--FirstPolicyLoad")]
        public bool FirstPolicyLoad { get; set; }

        [JsonProperty(PropertyName = "integer--AppLoginCount")]
        public int AppLoginCount { get; set; }
    }

    

   
}