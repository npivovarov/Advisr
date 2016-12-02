using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Advisr.Web.Models.AutopilotModels
{
    [DataContract]
    public class AutopilotLists
    {
        [JsonProperty(PropertyName = "lists")]
        public List<AutopilotList> Lists { get; set; }
    }
}