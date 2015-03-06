#region
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using bscheiman.Common.Aspnet.Interfaces;

#endregion

namespace bscheiman.Common.Aspnet.Database {
    public class BaseEntity : BaseEntity<long> {
    }

    public class BaseEntity<T> : IHasUsers, IHasDates, ISoftDelete {
        [Editable(false), HiddenInput(DisplayValue = false)]        
        public T Id { get; set; }

        [Editable(false), HiddenInput(DisplayValue = false)]        
        public DateTime DateCreated { get; set; }

        [Editable(false), HiddenInput(DisplayValue = false)]
        public DateTime DateModified { get; set; }

        [Editable(false), HiddenInput(DisplayValue = false)]
        public string UserCreated { get; set; }

        [Editable(false), HiddenInput(DisplayValue = false)]
        public string UserModified { get; set; }

        [Editable(false), HiddenInput(DisplayValue = false)]
        public bool IsDeleted { get; set; }
    }
}