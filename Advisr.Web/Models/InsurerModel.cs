using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Advisr.Web.Models
{
    public class InsurerModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Insurer Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Insurer URL")]
        public string URL { get; set; }

        public string  Email { get; set; }

        [Required]
        [Display(Name = "Insurer phone")]
        public string Phone { get; set; }

        [Display(Name = "Insurer phone overseas")]
        public string PhoneOverseas { get; set; }

        [Required]
        public Guid? LogoId { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }
    }
}