using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class Home_P : IBaseEntity
    {
        public int Id { get; set; }

        public int PolicyId { get; set; }

        public int AddressId { get; set; }
        
        public DateTime BuildDate { get; set; }
        
        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }


        //----------------------
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }

        [ForeignKey("PolicyId")]
        public virtual Policy Policy { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
    }
}
