using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class PolicyTypeField : IBaseEntity
    {
        public int Id { get; set; }

        public int PolicyTypeId { get; set; }

        public string FieldName { get; set; }

        public PolicyTypeFieldType FieldType { get; set; }

        public string DefaultValue { get; set; }

        public string ListDesription { get; set; }

        public FieldStatus? Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }



        //-----------------------------------
        [ForeignKey("PolicyTypeId")]
        public virtual PolicyType PolicyType { get; set; }
        
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }

    public enum PolicyTypeFieldType
    {
        String = 0,
        Int = 1,
        Bool = 2,
        Float = 3,
        List = 4
    }

    public enum FieldStatus
    {
        Active = 0, 
        Deleted
    }
}
