using System;

namespace Advisr.Domain.DbModels
{
    public class PolicyFile
    {
        public int Id { get; set; }

        public int PolicyId { get; set; }

        public Guid FileId { get; set; }

        public virtual File File { get; set; }

        public virtual Policy Policy { get; set; }

        public FileStatus Status { get; set; }
    }

    public enum FileStatus
    {
        active,
        deleted
    }
}
