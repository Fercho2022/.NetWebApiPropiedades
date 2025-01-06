namespace WebApiPropiedades.Exceptions
{
    public class NotFoundException : CustomException
    {
        public NotFoundException(string message) : base(message, StatusCodes.Status404NotFound)
        { }
    }
}
