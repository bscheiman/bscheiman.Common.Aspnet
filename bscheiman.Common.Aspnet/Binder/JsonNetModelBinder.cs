#region
using System.IO;
using System.Web.Mvc;
using bscheiman.Common.Extensions;

#endregion

namespace bscheiman.Common.Aspnet.Binder {
    public class JsonNetModelBinder<T> : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            var request = controllerContext.HttpContext.Request;
            request.InputStream.Seek(0, SeekOrigin.Begin);

            return new StreamReader(request.InputStream).ReadToEnd().FromJson<T>();
        }
    }
}