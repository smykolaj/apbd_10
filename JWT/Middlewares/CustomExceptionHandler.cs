using System.Net;

namespace JWT.Middlewares;

public class CustomExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandler> _logger;
    public CustomExceptionHandler(RequestDelegate next, ILogger<CustomExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    // Implement exception handling here
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Call the next middleware in the pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An unhandled exception occurred");

            // Handle the exception
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Set the status code and response content
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";

        // Create a response model
        var response = new
        {
            error = new
            {
                message = "An error occurred while processing your request.",
                detail = exception.Message
            }
        };

        // Serialize the response model to JSON
        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);

        // Write the JSON response to the HTTP response
        return context.Response.WriteAsync(jsonResponse);
    }
}