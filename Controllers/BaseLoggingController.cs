#region
using System.Data.Entity;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using bscheiman.Common.Aspnet.Extensions;
using bscheiman.Common.Aspnet.Objects;
using bscheiman.Common.Aspnet.Results;
using Microsoft.Owin;
using Microsoft.Owin.Security;

#endregion

namespace bscheiman.Common.Aspnet.Controllers {
    public class BaseLoggingController<TDatabase> : BaseLoggingController where TDatabase : DbContext, new() {
        protected TDatabase Database { get; set; }

        public BaseLoggingController() {
            Database = new TDatabase();
        }

        protected TDatabase GetContext() {
            return Database;
        }
    }

    public class BaseLoggingController : Controller {
        public IAuthenticationManager Authentication {
            get { return CurrentOwinContext.Authentication; }
        }

        public IOwinContext CurrentOwinContext {
            get { return HttpContext.GetOwinContext(); }
        }

        public string FacebookAccessToken {
            get {
                var claim = Identity.FindFirst("urn:facebook:access_token");

                return claim == null ? null : claim.Value;
            }
        }

        public ClaimsIdentity Identity {
            get { return Authentication.User.Identity as ClaimsIdentity; }
        }

        public new ClaimsPrincipal User {
            get { return Authentication.User; }
        }

        protected ActionResult ErrorJson(int code, string message) {
            return Json(new BaseJson {
                Valid = false,
                Message = message,
                Code = 0
            }, JsonRequestBehavior.AllowGet);
        }

        protected new ActionResult Json(object data, JsonRequestBehavior behavior = JsonRequestBehavior.DenyGet) {
            return new JsonNetResult {
                Data = data
            };
        }

        protected override void OnActionExecuting(ActionExecutingContext ctx) {
            ctx.Log();

            base.OnActionExecuting(ctx);
        }

        protected ActionResult SuccessJson(int code = 0, string message = "") {
            return Json(new BaseJson {
                Valid = true,
                Message = message,
                Code = 0
            }, JsonRequestBehavior.AllowGet);
        }
    }
}