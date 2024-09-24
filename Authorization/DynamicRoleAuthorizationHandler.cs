using Microsoft.AspNetCore.Authorization;

public class AuthorizeDynamicRolesAttribute : AuthorizeAttribute
{
    public AuthorizeDynamicRolesAttribute()
    {
        // This will use the dynamic role policy we set up
        Policy = "DynamicRolePolicy";
    }
}
