using Advisr.Domain.DbModels;
using System.Collections.Generic;

namespace Advisr.Web.Models
{
    public class PolicyTypeModel
    {
        public int PolicyTypeId { get; set; }

        public string PolicyGroupName { get; set; }

        public PolicyGroupType PolicyGroupType { get; set; }

        public string PolicyTypeName { get; set; }

        public PolicyTypeStatus Status { get; set; }

        public int InsurerId { get; set; }
    }
}