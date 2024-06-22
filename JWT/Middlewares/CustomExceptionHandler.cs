namespace JWT.Middlewares;

public class CustomExceptionHandler
{
    private readonly RequestDelegate _next;
    public CustomExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }
    
    // Implement exception handling here
    public async Task InvokeAsync(HttpContext context)
    {
        // Code to execute before the next middleware
        Console.WriteLine($"Request: {context.Request.Method} {context.Request.Path}");
        
        await _next(context);
        
        // Code to execute after the next middleware
        Console.WriteLine($"Response: {context.Response.StatusCode}");
    }
}