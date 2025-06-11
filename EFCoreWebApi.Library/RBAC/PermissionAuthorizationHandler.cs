namespace EFCoreWebApi.Library
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionAttribute>
    {
        IHttpContextAccessor HttpContextAccessor;
 
        public PermissionAuthorizationHandler(IHttpContextAccessor HttpContextAccessor)
        {
            this.HttpContextAccessor = HttpContextAccessor;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAttribute requirement)
        {
            var User = context.User;
            bool IsAuthenticated = User != null && User.Identity != null ? User.Identity.IsAuthenticated : false;

            if (IsAuthenticated)
            {
                // the Sub claim is NOT included in User.Claims
                // string Id = Tokens.GetClaimValue(context.User.Claims, JwtRegisteredClaimNames.Sub);

                // read the JwtRegisteredClaimNames.Sub from Token in HTTP request header
                HttpContext HttpContext = HttpContextAccessor.HttpContext;
                JwtSecurityToken JwtToken = Tokens.ReadTokenFromRequestHeader(HttpContext);
 
                string Id = Tokens.GetClaimValue(JwtToken.Claims, JwtRegisteredClaimNames.Sub);

                if (!string.IsNullOrWhiteSpace(Id))
                {
                    List<AppPermission> Permissions = RBAC.GetClientPermissions(Id);

                    foreach (var Permission in Permissions)
                    {
                        if (string.Compare(Permission.Name, requirement.PermissionName, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            context.Succeed(requirement);
                            break;
                        }
                    }
                }
 
            }

            return Task.CompletedTask;
        }
    }
}
