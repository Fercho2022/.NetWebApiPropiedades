namespace WebApiPropiedades.Exceptions
{
    public class BadRequestException : CustomException
    {
        public BadRequestException(string message) : base(message, StatusCodes.Status400BadRequest)
        { }
    }
}
