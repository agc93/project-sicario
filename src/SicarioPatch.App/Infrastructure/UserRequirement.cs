using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SicarioPatch.App.Infrastructure
{
    public abstract class UserAuthorization
    {
        private protected readonly AccessOptions _opts;
        private readonly Func<AccessOptions, List<string>> _selector;
        public const string UserIdClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        public const string DiscriminatorClaim = "urn:discord:user:discriminator";

        protected UserAuthorization(AccessOptions opts)
        {
            _opts = opts;
        }

        protected UserAuthorization(AccessOptions opts, Func<AccessOptions, List<string>> userFunc) : this(opts)
        {
            _selector = userFunc;
        }
        
        public bool AllowAllAuthenticated => _opts != null && _selector(_opts).Any(u => u == "*");

        public bool AllowsUser(ClaimsPrincipal principal)
        {
            return _opts != null
                   && _selector != null
                   && _selector(_opts).Any(u =>
                       {
                           var user = u.ToLower();
                           return user.All(char.IsDigit)
                               ? user == principal.FindFirst(UserIdClaim)?.Value?.ToLower()
                               : user.Contains("#")
                                    ? user == $"{principal.Identity?.Name?.ToLower()}#{principal.FindFirst(DiscriminatorClaim)?.Value}"
                                    : user == principal.Identity?.Name?.ToLower();
                       });
        }
    }
    public class UserRequirement : UserAuthorization, IAuthorizationRequirement
    {
        public UserRequirement(AccessOptions opts) : base(opts, o => o.AllowedUsers)
        {
        }

        public bool AllowAll => _opts != null && _opts.AllowedUsers.Any(u => u == "**");

        public List<string> AllowedUsers => _opts.AllowedUsers;
    }

    public class UploaderRequirement : UserAuthorization, IAuthorizationRequirement
    {
        public UploaderRequirement(AccessOptions opts): base(opts, o => o.AllowedUploaders)
        {
        }

        public List<string> AllowedUsers => _opts.AllowedUploaders;
    }

    public class UserAccessHandler : AuthorizationHandler<UserRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
        {
            if (requirement.AllowAll)
            {
                context.Succeed(requirement);
            }
            else if (requirement.AllowAllAuthenticated && !string.IsNullOrWhiteSpace(context.User.Identity?.Name))
            {
                context.Succeed(requirement);
            } else if (requirement.AllowsUser(context.User))
            {
                context.Succeed(requirement);
            }
        }
    }
    
    public class UploadAccessHandler : AuthorizationHandler<UploaderRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UploaderRequirement requirement)
        {
            if (requirement.AllowAllAuthenticated)
            {
                context.Succeed(requirement);
            } else if (context.User.Identity?.Name != null && requirement.AllowsUser(context.User))
            {
                context.Succeed(requirement);
            }
        }
    }
}

namespace SicarioPatch.App {
    public static class Policies
    {
        public const string IsUser = "IsUser";
        public const string IsUploader = "IsUploader";
    }
}