using Advisr.Domain.DbModels;
using System.ComponentModel.DataAnnotations;

namespace Advisr.Web.Models
{

    public class CoverageModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public CoverageType Type { get; set; }
    }

    public class NewCoverageModel : CoverageModel
    {
        [Required]
        public int InsurerId { get; set; }
        
        public int? PolicyTypeId { get; set; }
    }

    public class EditCoverageModel : CoverageModel
    {
        [Required]
        public int Id { get; set; }
    }


    public class AssignCoverageModel
    {
        [Required]
        public int CoverageId { get; set; }

        [Required]
        public int PolicyTypeId { get; set; }
    }


}