#region
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace bscheiman.Common.Aspnet.Extensions {
    public static class IQueryableExtensions {
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] paths) where T : class {
            return paths.Aggregate(query, (current, path) => current.Include(path));
        }
    }
}