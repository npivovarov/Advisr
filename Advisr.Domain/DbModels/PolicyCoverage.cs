using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class PolicyCoverage : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        public int PolicyId { get; set; }
        
        public int CoverageId { get; set; }

        public bool IsActive { get; set; }

        public Nullable<decimal> Excess { get; set; }
        
        public Nullable<decimal> MaxPayAmount { get; set; }

        public string Limit { get; set; }
        

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }


        //-------------
        public virtual Policy Policy { get; set; }

        public virtual Coverage Coverage { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }


    }
}
