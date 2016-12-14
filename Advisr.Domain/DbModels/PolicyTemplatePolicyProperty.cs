using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advisr.Domain.DbModels
{
    public class PolicyTemplatePolicyProperty : IBaseEntity
    {
        public int Id { get; set; }

        public int PolicyTemplateId { get; set; }

        public int PolicyPropertyId { get; set; }

        public int Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedById { get; set; }

        public string ModifiedById { get; set; }
        
        //----------------------
        [ForeignKey("PolicyTemplateId")]
        public virtual PolicyTemplate PolicyTemplate { get; set; }

        [ForeignKey("PolicyPropertyId")]
        public virtual PolicyProperty PolicyProperty { get; set; }
        
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        [ForeignKey("ModifiedById")]
        public virtual ApplicationUser ModifiedBy { get; set; }
    }
}
