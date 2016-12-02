namespace Advisr.Domain.DbModels
{
    public class VehicleModel
    {
        public int Id { get; set; }

        public int VehicleMakeId { get; set; }

        public string ModelName { get; set; }

        public int Status { get; set; }

        public virtual VehicleMake VehicleMake { get; set; }
    }
}
