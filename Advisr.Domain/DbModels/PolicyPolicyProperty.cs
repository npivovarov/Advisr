using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class PolicyPolicyProperty
    {
        public int Id { get; set; }

        public int PolicyId { get; set; }

        public int PolicyPropertyId { get; set; }

        public string Value { get; set; }
        
        [ForeignKey("PolicyId")]
        public virtual Policy Policy { get; set; }

        [ForeignKey("PolicyPropertyId")]
        public virtual PolicyProperty PolicyProperty { get; set; }
    }
}
