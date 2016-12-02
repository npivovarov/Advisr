using System;
using System.ComponentModel.DataAnnotations;

namespace Advisr.Web.Models
{
    public class CustomerAddressModel
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public int Id { get; set; }
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
    }
}