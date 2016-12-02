using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace Advisr.Domain.DbModels
{
    public class Address : IBaseEntity
    {
        public int Id { get; set; }

        public string Address_1 { get; set; }

        public string Address_2 { get; set; }

        public string Country { get; set; }

        public string City { get; set; }

        public string Street { get; set; }

        public string State { get; set; }

        public string PostCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }
        
        /// -----------
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }
}
