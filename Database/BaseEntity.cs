#region
using System;
using bscheiman.Common.Aspnet.Interfaces;

#endregion

namespace bscheiman.Common.Aspnet.Database {
    public class BaseEntity : IHasUsers, IHasDates, ISoftDelete {
        public long Id { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserCreated { get; set; }
        public string UserModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}