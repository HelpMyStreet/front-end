using Microsoft.AspNetCore.Mvc.Filters;

namespace HelpMyStreetFE.Helpers
{
    public class AuthorizeAttributeNoRedirect : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.User == null || !filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
            }
        }
    }
}
