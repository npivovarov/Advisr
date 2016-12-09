using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class PolicyTypeCoverage : IBaseEntity
    {
        [Key]
        [Column(Order = 1)]
        public int PolicyTypeId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int CoverageId { get; set; }
        
        public PolicyTypeCoverageStatus Status { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }

        
        //-----------------------------------
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
        
        [ForeignKey("PolicyTypeId")]
        public virtual PolicyType PolicyType { get; set; }

        [ForeignKey("CoverageId")]
        public virtual Coverage Coverage { get; set; }
    }

    public enum PolicyTypeCoverageStatus
    {
        None = 0,
    }
}
