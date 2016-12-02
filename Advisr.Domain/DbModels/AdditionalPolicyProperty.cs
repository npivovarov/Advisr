using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advisr.Domain.DbModels
{
    public class AdditionalPolicyProperty
    {
        public int Id { get; set; }

        public int PolicyId { get; set; }

        public int PolicyTypeFieldId { get; set; }

        public string Value { get; set; }
        
        [ForeignKey("PolicyId")]
        public virtual Policy Policy { get; set; }

        [ForeignKey("PolicyTypeFieldId")]
        public virtual PolicyTypeField PolicyTypeField { get; set; }
    }
}
