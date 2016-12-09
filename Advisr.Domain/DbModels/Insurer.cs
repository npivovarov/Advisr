using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace Advisr.Domain.DbModels
{
    public class Insurer : IBaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string URL { get; set; }

        public string Phone { get; set; }

        public string PhoneOverseas { get; set; }

        public string Email { get; set; }

        public int? AddressId { get; set; }

        public Guid? LogoFileId { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }

        public string Color { get; set; }

        public string Description { get; set; }

        public InsurerStatus  Status { get; set; }

        /// -----------
        public virtual ICollection<PolicyType> PolicyGroups { get; set; }

        [ForeignKey("LogoFileId")]
        public virtual File LogoFile { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }

    public enum InsurerStatus
    {
        Active,
        Deleted
    }
}
