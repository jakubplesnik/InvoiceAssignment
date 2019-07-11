using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvoiceAssignment.Auth
{
    public class UsersOnlyAuthorizationHandler : AuthorizationHandler<UsersOnlyRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UsersOnlyRequirement requirement)
        {
            if (context.User.IsInRole("User"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
