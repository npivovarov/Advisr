using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advisr.Domain.DbModels
{
    public class PolicyProperty : IBaseEntity
    {
        public int Id { get; set; }
        
        public string FieldName { get; set; }

        public PolicyTypeFieldType FieldType { get; set; }

        public int OrderIndex { get; set; }

        public PropertyType PropertyType { get; set; }
        
        public bool IsRequired { get; set; }

        public bool IsTitle { get; set; }

        public bool IsSubtitle { get; set; }
        
        public string DefaultValue { get; set; }

        public string ListDesription { get; set; }

        public FieldStatus? Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }
        
        //-----------------------------------
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
        List = 4,
        Date = 5
    }

    public enum FieldStatus
    {
        Active = 0,
        Deleted
    }

    public enum PropertyType
    {
        None = -1,
        PolicyTemplate = 0,
        AdditionalToPolicyType = 1
    }

}
