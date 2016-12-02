using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class CustomerAddress : IBaseEntity
    {
        [Key]
        [Column(Order = 1)]
        public Guid CustomerDetailsId { get; set; }

        [Key]
        [Column(Order = 2)]
        public int AddressId { get; set; }

        public int Status { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }

        //-------------
        [ForeignKey("CustomerDetailsId")]
        public virtual CustomerDetails CustomerDetails { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }
}
