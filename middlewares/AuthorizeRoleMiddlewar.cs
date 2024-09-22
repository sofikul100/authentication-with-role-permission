using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json;

public class AthorizeRoleMiddleware
{
    private readonly RequestDelegate _next;

    public AthorizeRoleMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        // Check if the response status code is 403
        if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
        {
            context.Response.ContentType = "application/json";

            // Get the user's roles from claims
            var userRoles = context.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var response = new
            {
                Success = false,
                StatusCode = context.Response.StatusCode,
                Error = "Access denied. You do not have the required role to access this resource.",
                Details = "You do not have permission to perform this action.",
                Roles = userRoles // Include the user's roles
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
