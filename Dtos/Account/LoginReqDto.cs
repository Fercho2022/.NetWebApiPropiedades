using Microsoft.AspNetCore.Authentication;

namespace WebApiPropiedades.Dtos.Account
{
    public class LoginReqDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
