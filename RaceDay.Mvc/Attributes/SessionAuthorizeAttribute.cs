using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RaceDay.Mvc.Attributes;

public class SessionAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string _role;

    public SessionAuthorizeAttribute(string role = "") => _role = role;

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var userRole = context.HttpContext.Session.GetString("UserRole");

        if (string.IsNullOrEmpty(userRole))
        {
            context.Result = new RedirectToActionResult("Login", "Account", null);
        }
        else if (!string.IsNullOrEmpty(_role) && userRole != _role)
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}