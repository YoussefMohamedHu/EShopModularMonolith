using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Base.Pagination
{
    public class PaginatedResult<TEntity>(int pageIndex, int pageSize, int totalCount, IEnumerable<TEntity> items) where TEntity : class
    {
        public int PageIndex { get; } = pageIndex;
        public int PageSize { get; } = pageSize;
        public int TotalCount { get; } = totalCount;
        public IEnumerable<TEntity> Items { get; } = items;
    }
}
