using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class PolicyTemplate : IBaseEntity
    {
        public int Id { get; set; }

        public int PolicyGroupId { get; set; }

        public string Name { get; set; }

        public PolicyTemplateStatus Status { get; set; }
        
        public string Title { get; set; }

        public string Subtitle { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }

        //----------------------
        public virtual ICollection<PolicyTemplatePolicyProperty> PolicyTemplatePolicyProperties { get; set; }

        [ForeignKey("PolicyGroupId")]
        public virtual PolicyGroup PolicyGroup { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }

    }

    public enum PolicyTemplateStatus
    {
        None = 0,
        Deleted = 1
    }
}
