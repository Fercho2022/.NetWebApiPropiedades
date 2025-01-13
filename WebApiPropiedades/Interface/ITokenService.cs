using WebApiPropiedades.Models;

namespace WebApiPropiedades.Interface
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
