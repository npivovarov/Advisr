using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Advisr.Web.Models
{
    public class AddFilesModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public List<Guid> FileIds { get; set; }
    }
}