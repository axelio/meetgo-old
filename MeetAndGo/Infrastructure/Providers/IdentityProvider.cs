using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MeetAndGo.Infrastructure.Providers
{
    public interface IIdentityProvider
    {
        string GetUserIdFromClaims();
        string GetUserMailFromClaims();
        IDictionary<string, string> GetClaims();
    }

    public class IdentityProvider : IIdentityProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public IdentityProvider(IHttpContextAccessor contextAccessor) => _contextAccessor = contextAccessor;

        public string GetUserIdFromClaims() => _contextAccessor?.HttpContext?.User.Claims
                .FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.NameIdentifier))?.Value;

        public string GetUserMailFromClaims() => _contextAccessor?.HttpContext?.User.Claims
                .FirstOrDefault(c => string.Equals(c.Type, ClaimTypes.Email))?.Value;

        public IDictionary<string, string> GetClaims() =>
            _contextAccessor?.HttpContext?.User.Claims.ToDictionary(c => c.Type, c => c.Value);
    }
}
