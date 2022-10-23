using MeetAndGo.Infrastructure.Providers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeetAndGo.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityProvider _identityProvider;

        public AuthController(IIdentityProvider identityProvider)
        {
            _identityProvider = identityProvider;
        }

        [HttpGet]
        [Route("me")]
        public IActionResult GetUserInfo()
        {
            return Ok(_identityProvider.GetClaims());
        }
    }
}
