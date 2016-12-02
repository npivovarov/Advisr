using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    /// <summary>
    /// Samples https://www.farmers.com/auto/coverage/
    /// </summary>
    public class Coverage : IBaseEntity
    {
        public int Id { get; set; }
        
        public int InsurerId { get; set; }

        public CoverageType Type { get; set; }

        public string Title { get; set; }
        
        public string Description { get; set; }

        public CoverageStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }


        //----------------------
        [ForeignKey("InsurerId")]
        public virtual Insurer Insurer { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }

    public enum CoverageType
    {
        StandardCover = 0,
        OptionalCover = 1,
        ExclusionsCover = 2,
    }

    public enum CoverageStatus
    {
        None = 0,
        Hidden = 1
    }
}
