using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class Policy : IBaseEntity
    {
        public int Id { get; set; }
        
        public int? PolicyTypeId { get; set; }
        
        public int? InsurerId { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string PolicyNumber { get; set; }

        public string PrePolicyType { get; set; }
        
        public DateTime? PolicyEffectiveDate { get; set; }
        
        //public DateTime? PolicyExpiryDate { get; set; }
        
        public decimal? PolicyPremium { get; set; }
        
        public PolicyPaymentFrequency PolicyPaymentFrequency { get; set; }
        
        public decimal? PolicyPaymentAmount { get; set; }
        
        public decimal? PolicyExcess { get; set; }

        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }        

        public PolicyStatus Status { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public DateTime? ProcessedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }

        public string ProcessedById { get; set; }


        //-----------------------------------

        [ForeignKey("PolicyTypeId")]
        public virtual PolicyType PolicyType { get; set; }

        [ForeignKey("InsurerId")]
        public virtual Insurer Insurer { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }

        [ForeignKey("ProcessedById")]
        public virtual ApplicationUser ProcessedBy { get; set; }
        
        public virtual ICollection<PolicyFile> PolicyFiles { get; set; }

        public virtual ICollection<PolicyPolicyProperty> PolicyPolicyProperties { get; set; }

        public virtual ICollection<PolicyCoverage> PolicyCoverages { get; set; }

        public virtual ICollection<UserPolicy> PolicyUsers { get; set; }
    }
    
    public enum PolicyStatus
    {
        None = -1,
        Unconfirmed = 0,
        Confirmed = 1,
        Rejected = 2,
        Hidden = 3
    }

    public enum PolicyPaymentFrequency
    {
        None = 0,
        Weekly = 1,
        Fortnightly = 2,
        Monthly = 3,
        Annual = 4
    }
}
