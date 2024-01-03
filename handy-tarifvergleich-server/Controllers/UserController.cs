using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handy_tarifvergleich_server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("users")]
    public class UserController : Controller
    {
        
    }
}
