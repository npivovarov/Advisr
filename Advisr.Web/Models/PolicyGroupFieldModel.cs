using Advisr.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Advisr.Web.Models
{
    public class PolicyGroupFieldModel
    {
        public int FieldId { get; set; }

        [Required]
        public int PolicyTypeId { get; set; }

        [Required]
        public string FieldName { get; set; }

        [Required]
        public PolicyTypeFieldType FieldType { get; set; }

        public string DefaultValue { get; set; }

        public string ListDescription { get; set; }
    }
}