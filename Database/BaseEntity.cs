#region
using System;
using System.ComponentModel.DataAnnotations;
using bscheiman.Common.Aspnet.Interfaces;

#endregion

namespace bscheiman.Common.Aspnet.Database {
    public class BaseEntity : BaseEntity<long> {
    }

    public class BaseEntity<T> : IHasUsers, IHasDates, ISoftDelete {
        [ScaffoldColumn(false)]
        public T Id { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateCreated { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateModified { get; set; }

        [ScaffoldColumn(false)]
        public string UserCreated { get; set; }

        [ScaffoldColumn(false)]
        public string UserModified { get; set; }

        [ScaffoldColumn(false)]
        public bool IsDeleted { get; set; }
    }
}