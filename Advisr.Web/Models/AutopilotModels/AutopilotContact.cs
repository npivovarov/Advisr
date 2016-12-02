using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Advisr.Web.Models.AutopilotModels
{
    [DataContract]
    public class AutopilotContactToAdd
    {
        [JsonProperty(PropertyName = "contact")]
        public Contact Contact { get; set; }
    }

    public class AutopilotContactId
    {
        [JsonProperty(PropertyName ="contact_id")]
        public string ContactId { get; set; }
    }

    public class AutopilotContact
    {
        public string Email { get; set; }

        [JsonProperty(PropertyName = "custom_fields")]
        public List<CustomField> CustomFields { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string LeadSource { get; set; }

        [JsonProperty(PropertyName = "lists")]
        public List<string> Lists { get; set; }

        [JsonProperty(PropertyName = "contact_id")]
        public string Id { get; set; }
    }

    public class CustomField
    {
        [JsonProperty(PropertyName = "kind")]
        public string Kind { get; set; }

        [JsonProperty(PropertyName = "value")]
        public object Value { get; set; }

        [JsonProperty(PropertyName = "fieldType")]
        public string FieldType { get; set; }

        [JsonProperty(PropertyName = "deleted")]
        public bool Deleted { get; set; }
    }
}