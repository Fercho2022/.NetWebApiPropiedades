using System.Text.Json;
using WebApiPropiedades.Exceptions;

namespace WebApiPropiedades.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";

               
                switch (error)
                {
                    case CustomException e:
                        // Custom error
                        response.StatusCode = e.StatusCode;
                        break;
                    case KeyNotFoundException e:
                        // Not found error
                        response.StatusCode = StatusCodes.Status404NotFound;
                        break;
                    default:
                        // Unhandled error
                        _logger.LogError(error, error.Message);
                        response.StatusCode = StatusCodes.Status500InternalServerError;
                        break;
                }

                var result = JsonSerializer.Serialize(new
                {
                    StatusCode = response.StatusCode,
                    Message = error?.Message ?? "Internal Server Error",
                    // Solo incluir StackTrace en desarrollo
                    StackTrace = context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment()
                        ? error?.StackTrace
                        : null
                });

                await response.WriteAsync(result);
            }
        }
    }
}
