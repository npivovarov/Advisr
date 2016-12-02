using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class Vehicle_P : IBaseEntity
    {
        public int Id { get; set; }

        public int PolicyId { get; set; }

        //public string PolicyHolderId { get; set; }

        public int Year { get; set; }

        public int MakeId { get; set; }

        public int ModelId { get; set; }

        public string Colour { get; set; }

        public int Mileage { get; set; }

        public string RegistredDriverName { get; set; }

        public string RegistrationNumber { get; set; }
        
        public string RegistrationState { get; set; }
        
        public bool IsCommercial { get; set; }
        
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

        [ForeignKey("MakeId")]
        public virtual VehicleMake Make { get; set; }
        
        [ForeignKey("ModelId")]
        public virtual VehicleModel Model { get; set; }
    }
}
