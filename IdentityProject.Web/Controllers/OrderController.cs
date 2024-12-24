using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.Web.Controllers
{
    public class OrderController : Controller
    {
        [Authorize(Policy = "OrderReadPermission")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
