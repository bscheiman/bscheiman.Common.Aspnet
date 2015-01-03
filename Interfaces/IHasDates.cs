#region
using System;

#endregion

namespace bscheiman.Common.Aspnet.Interfaces {
    public interface IHasDates {
        DateTime? DateCreated { get; set; }
        DateTime? DateModified { get; set; }
    }
}