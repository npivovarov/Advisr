using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Advisr.Web.Models.AutopilotModels
{
    public class AutopilotTrigger
    { 
        [JsonProperty(PropertyName = "trigger_id")]
        public string TriggerId { get; set; }

        [JsonProperty(PropertyName = "journey")]
        public string JourneyName { get; set; }

        [JsonProperty(PropertyName = "triggerType")]
        public string TriggerType { get; set; }
    }
}