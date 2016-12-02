using Advisr.Domain.DbModels;
using System.Collections.Generic;

namespace Advisr.Web.Models
{
    public class PolicyGroupModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public string SubgroupName { get; set; }

        public PolicyGroupType GroupType { get; set; }

        public Status Status { get; set; }

        public int InsurerId { get; set; }

        public List<PolicyGroupFieldModel> PolicyGroupFields { get; set; }
    }
}