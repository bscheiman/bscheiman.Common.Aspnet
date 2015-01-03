#region
using System.Linq;
using System.Web.Mvc;

#endregion

namespace bscheiman.Common.Aspnet.Extensions {
    public static class ModelStateExtensions {
        public static string[] GetErrors(this ModelStateDictionary modelState) {
            return modelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToArray();
        }
    }
}