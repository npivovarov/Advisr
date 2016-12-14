using Advisr.Domain.DbModels;
using System.Collections.Generic;

namespace Advisr.Web.Models
{
    public class PolicyTypeModel
    {
        public int PolicyTypeId { get; set; }

        public int PolicyGroupId { get; set; }

        public int PolicyTemplateId { get; set; }

        public string PolicyTypeName { get; set; }

        public PolicyTypeStatus Status { get; set; }

        public int InsurerId { get; set; }

        public string Description { get; set; }
    }
}