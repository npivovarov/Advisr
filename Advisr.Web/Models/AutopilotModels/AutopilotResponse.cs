using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Advisr.Web.Models.AutopilotModels
{
    public class AutopilotResponse<T>
    {
        public T Result { get; set; }
        public AutopilotError Error { get; set; }

        public bool HasError { get; set; }
    }

    public class AutopilotError
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
    }
}