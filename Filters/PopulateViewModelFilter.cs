#region
using System.Web.Mvc;
using bscheiman.Common.Aspnet.ViewModel;

#endregion

namespace bscheiman.Common.Aspnet.Filters {
    public class PopulateViewModelFilter : ActionFilterAttribute {
        public override void OnActionExecuted(ActionExecutedContext filterContext) {
            BaseViewModel model;

            if (filterContext.Controller.ViewData.Model == null) {
                model = new BaseViewModel();
                filterContext.Controller.ViewData.Model = model;
            } else
                model = filterContext.Controller.ViewData.Model as BaseViewModel;

            if (model != null) {
                model.IsAuthenticated = filterContext.HttpContext.User.Identity.IsAuthenticated;

                if (model.IsAuthenticated)
                    model.UserName = filterContext.HttpContext.User.Identity.Name;
            }

            base.OnActionExecuted(filterContext);
        }
    }
}