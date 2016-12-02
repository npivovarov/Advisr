using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class CustomerDetails : IBaseEntity
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string UserId { get; set; }
        
        public int AddressId { get; set; }
        public string ContactPhone { get; set; }

        public string HomePhone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public MaritalStatus? MaritalStatus { get; set; }
        
        public Gender? Gender { get; set; }

        public CustomerStatus Status { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }

        [Required]
        public string ModifiedReason { get; set; }

        //----------------------
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
        //public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }

    public enum MaritalStatus
    {
        Single = 0,
        Married = 1
    }

    public enum Gender
    {
        Male = 0,
        Female = 1
    }

    public enum CustomerStatus
    {
        Pending = 0,
        Completed = 1
    }
}
