using System.Collections.Generic;

namespace Advisr.Domain.DbModels
{
    public class VehicleMake
    {
        public int Id { get; set; }

        public string MakeName { get; set; }

        public string Description { get; set; }

        public int Status { get; set; }
        
        public virtual ICollection<VehicleModel> VehicleModels { get; set; }
    }
}
