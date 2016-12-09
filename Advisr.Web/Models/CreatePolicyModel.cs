using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Advisr.Web.Models
{
    public class CreatePolicyModel
    {
        [Required]
        public string PrePolicyType { get; set; }

        [Required]
        public List<Guid> FileIds { get; set; }

    }
}