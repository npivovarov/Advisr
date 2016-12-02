using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Advisr.Domain.DbModels
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public long FileSize { get; set; }

        public string Description { get; set; }
        
        public string ContentType { get; set; }

        public string LocationPath { get; set; }

        public bool IsTemp { get; set; }

        public DateTime CreatedDate { get; set; }

        public string CreatedById { get; set; }
    }
}
