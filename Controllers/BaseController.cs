using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SmartStudyPlanner.Controllers;

public abstract class BaseController : Controller
{
    protected int CurrentUserId
    {
        get
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null && int.TryParse(claim.Value, out int id))
            {
                return id;
            }
            return 0;
        }
    }

    protected string CurrentUserName => User.FindFirst(ClaimTypes.Name)?.Value ?? "Student";
}

