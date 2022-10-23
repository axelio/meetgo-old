using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MeetAndGo.Authorization
{
    public class OnlyClientRequirementHandler : AuthorizationHandler<OnlyClientRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OnlyClientRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "Company"))
                return Task.CompletedTask;

            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}