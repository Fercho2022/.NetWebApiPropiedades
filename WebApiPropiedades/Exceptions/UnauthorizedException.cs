namespace WebApiPropiedades.Exceptions
{
    public class UnauthorizedException : CustomException
    {
        public UnauthorizedException(string message) : base(message, StatusCodes.Status401Unauthorized)
        { }
    }
}
