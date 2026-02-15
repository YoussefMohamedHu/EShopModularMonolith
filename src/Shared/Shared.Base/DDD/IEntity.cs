using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Base.DDD
{
    public interface IEntity<TId> : IEntity
    {
        TId Id { get; }
    }
    public interface IEntity
    {
        DateTime? CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTime? LastModifiedAt { get; set; }
        string? LastModifiedBy { get; set; }

    }
}
