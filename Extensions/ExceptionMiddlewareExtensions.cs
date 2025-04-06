using WebApiPropiedades.Exceptions;
using WebApiPropiedades.Middlewares;

namespace WebApiPropiedades.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static WebApplication UseCustomExceptionMiddleware(
                this WebApplication app)
        {
            
            return (WebApplication)app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
