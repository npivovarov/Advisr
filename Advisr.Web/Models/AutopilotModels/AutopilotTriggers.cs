using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Advisr.Web.Models.AutopilotModels
{
    public class AutopilotTriggers
    {
        [JsonProperty(PropertyName = "triggers")]
        public List<AutopilotTrigger> Triggers { get; set; }
    }
}