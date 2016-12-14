using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advisr.Domain.DbModels
{
    public class PolicyType : IBaseEntity
    {
        public int Id { get; set; }

        public int InsurerId { get; set; }

        public int PolicyTemplateId { get; set; }

        public int PolicyGroupId { get; set; }
        
        public string PolicyTypeName { get; set; }

        public string Description { get; set; }

        public PolicyTypeStatus? Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }


        //-----------------------------------
        public virtual ICollection<Policy> Policies { get; set; }
        
        public virtual ICollection<PolicyTypePolicyProperty> PolicyTypePolicyProperties { get; set; }

        [ForeignKey("PolicyTemplateId")]
        public virtual PolicyTemplate PolicyTemplate { get; set; }

        [ForeignKey("PolicyGroupId")]
        public virtual PolicyGroup PolicyGroup { get; set; }

        [ForeignKey("InsurerId")]
        public virtual Insurer Insurer { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }

    public enum PolicyGroupType
    {
        Vehicle = 0,
        Home = 1,
        Life = 2
    }

    public enum PolicyTypeStatus
    {
        Active = 0,
        Hide = 1
    }
}
