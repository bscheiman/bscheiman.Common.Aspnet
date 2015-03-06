#region
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

#endregion

namespace bscheiman.Common.Aspnet.Extensions {
    public static class IQueryableExtensions {
        public static IQueryable<T> IncludeAll<T>(this IQueryable<T> queryable) where T : class {
            var type = typeof (T);
            var properties = type.GetProperties();

            return (from property in properties
                    let isVirtual = property.GetGetMethod().IsVirtual
                    where isVirtual && properties.FirstOrDefault(c => c.Name == property.Name + "Id") != null
                    select property).Aggregate(queryable, (current, property) => current.Include(property.Name));
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] paths) where T : class {
            return paths.Aggregate(query, (current, path) => current.Include(path));
        }
    }
}