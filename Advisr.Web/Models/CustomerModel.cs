using Advisr.Domain.DbModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Advisr.Web.Models
{
    public class CustomerModel
    {
        [Required]
        public Guid Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Roles")]
        public List<string> Roles { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        public string HomePhone { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Marital status")]
        public MaritalStatus? MaritalStatus { get; set; }

        [Required]
        public Gender? Gender { get; set; }

        public CustomerAddressModel Address { get; set; }
        //[Display(Name = "Address 2")]
        //public CustomerAddressModel AddressSecond { get; set; }

        [Required]
        [Display(Name = "Reason of editing")]
        public string ModifiedReason { get; set; }
    }
}