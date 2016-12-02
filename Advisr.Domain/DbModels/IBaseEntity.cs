using System;

namespace Advisr.Domain.DbModels
{
    public interface IBaseEntity
    {
        DateTime CreatedDate { get; set; }

        DateTime? ModifiedDate { get; set; }

        string CreatedById { get; set; }

        string ModifiedById { get; set; }

    }
}
