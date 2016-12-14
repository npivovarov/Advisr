using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class PolicyTypePolicyProperty : IBaseEntity
    {
        public int Id { get; set; }

        public int PolicyTypeId { get; set; }

        public int PolicyPropertyId { get; set; }
        
        public PolicyTypePolicyPropertyStatus Status { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }
        
        //-----------------------------------
        [ForeignKey("PolicyTypeId")]
        public virtual PolicyType PolicyType { get; set; }

        [ForeignKey("PolicyPropertyId")]
        public virtual PolicyProperty PolicyProperty { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }

    public enum PolicyTypePolicyPropertyStatus
    {
        None = 0,
        Deleted = 1
    }

}
