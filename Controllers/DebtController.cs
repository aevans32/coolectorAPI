using Microsoft.AspNetCore.Mvc;

namespace CoolectorAPI.Controllers
{
    [Route("api/debt")]
    [ApiController]
    public class DebtController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
