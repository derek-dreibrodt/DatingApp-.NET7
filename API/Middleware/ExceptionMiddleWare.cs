using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware
{
    public class ExceptionMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleWare> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleWare(
            RequestDelegate next, // what is the next middleware to go to
            ILogger<ExceptionMiddleWare> logger, 
            IHostEnvironment env
        )
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(
            HttpContext context // gives access to  the http request going through
        ) 
        {
            try
            {
                await _next (context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // gives error code of 500
                var response = _env.IsDevelopment() // isdevelopment is found based on launchsettings.json
                    ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString()) // if development, return detailed error
                    : new ApiException(context.Response.StatusCode, ex.Message, "Internal server error"); // if not development, return vague error
                
                var options = new JsonSerializerOptions{PropertyNamingPolicy = JsonNamingPolicy.CamelCase}; // set json to camelcase for properties
                var json = JsonSerializer.Serialize(response, options); // Turn our response into a json file

                await context.Response.WriteAsync(json);
            }
        }
    }
}