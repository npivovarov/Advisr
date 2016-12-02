using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Advisr.Web.Models.AutopilotModels
{
    public class AutopilotList
    {
        [JsonProperty(PropertyName = "list_id")]
        public string ListId { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }
}