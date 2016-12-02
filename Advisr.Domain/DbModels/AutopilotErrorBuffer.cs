using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class AutopilotErrorBuffer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string  AutopilotContactId { get; set; }

        public string RequestUri { get; set; }

        public string RequestData { get; set; }

        public string OperationType { get; set; }

        public string UserId { get; set; }
    }
}
