using Newtonsoft.Json;
public class CustomUnauthorizedResponseMiddleware
{
    private readonly RequestDelegate _next;

    public CustomUnauthorizedResponseMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        await _next(context);

        if (context.Response.StatusCode == 401) // Unauthorized
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                error = "Unauthorized access",
                message = "You are not authorized to access this resource. Please log in First."
            }));
        }
    }
}
