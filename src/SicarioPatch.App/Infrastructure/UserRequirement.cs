using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SicarioPatch.App.Infrastructure
{
    public class UserRequirement : IAuthorizationRequirement
    {
        private AccessOptions _opts;

        public UserRequirement(AccessOptions opts)
        {
            _opts = opts;
        }

        public UserRequirement()
        {
            
        }

        public bool AllowAll => _opts != null && _opts.AllowedUsers.Any(u => u == "**");
        public bool AllowAllAuthenticated => _opts == null || _opts.AllowedUsers.Any(u => u == "*");

        public List<string> AllowedUsers => _opts.AllowedUsers;
    }

    public class UploaderRequirement : IAuthorizationRequirement
    {
        private AccessOptions _opts;

        public UploaderRequirement()
        {
            
        }

        public UploaderRequirement(AccessOptions opts)
        {
            _opts = opts;
        }

        public bool AllowAll => _opts == null || _opts.AllowedUploaders.Any(u => u == "*");

        public List<string> AllowedUsers => _opts.AllowedUploaders;
    }

    public class UserAccessHandler : AuthorizationHandler<UserRequirement>
    {
        // public static string UserPolicyName => "Users";
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement)
        {
            if (requirement.AllowAll)
            {
                context.Succeed(requirement);
            }
            else if (requirement.AllowAllAuthenticated && !string.IsNullOrWhiteSpace(context.User.Identity?.Name))
            {
                context.Succeed(requirement);
            } else if (requirement.AllowedUsers.Any(u => u == context.User.Identity?.Name))
            {
                context.Succeed(requirement);
            }
        }
    }
    
    public class UploadAccessHandler : AuthorizationHandler<UploaderRequirement>
    {
        // public static string UploadPolicy => "Uploaders";
        
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, UploaderRequirement requirement)
        {
            if (requirement.AllowAll)
            {
                context.Succeed(requirement);
            } else if (context.User.Identity?.Name != null && requirement.AllowedUsers.Any(u =>
                string.Equals(u.ToLower(), context.User.Identity.Name, StringComparison.InvariantCultureIgnoreCase)))
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