#region
using System;
using bscheiman.Common.Aspnet.Interfaces;

#endregion

namespace bscheiman.Common.Aspnet.Database {
    public class BaseEntity : BaseEntity<long> {
    }

    public class BaseEntity<T> : IHasUsers, IHasDates, ISoftDelete {
        public T Id { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public string UserCreated { get; set; }
        public string UserModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}