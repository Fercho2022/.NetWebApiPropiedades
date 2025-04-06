using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Services
{
    public class TokenService : ITokenService
    {
        //Esta variable se utiliza para acceder a la configuración de la
        //aplicación.

        private readonly IConfiguration _config;

        //Esta variable representa la clave de seguridad simétrica utilizada
        //para firmar y verificar los tokens JWT.

        private readonly SymmetricSecurityKey _Key;

        public TokenService(IConfiguration config)
        {
            _config = config;

            // inicializa la variable _Key con una instancia de SymmetricSecurityKey
            // utilizando la clave de firma obtenida de la configuración.

            _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

        // devuelve una cadena (string) que representa el token JWT generado.
        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {

                 // Agrega el ID del usuario como NameIdentifier
                 new Claim(ClaimTypes.NameIdentifier, user.Id),

                // Se crean claims (reclamaciones) que representan la
                // información asociada con el usuario. En este caso,
                // se incluyen el correo electrónico y el nombre de usuario
                // como reclamaciones.
                
               new Claim(JwtRegisteredClaimNames.Email, user.Email),
               new Claim(JwtRegisteredClaimNames.GivenName, user.UserName)

           };

            // Se crean credenciales de firma (SigningCredentials) utilizando
            // la clave _Key y el algoritmo de firma HmacSha512Signature.

            var creds = new SigningCredentials(_Key, SecurityAlgorithms.HmacSha512Signature);

            // Se crea un descriptor de token de seguridad (SecurityTokenDescriptor)
            // que especifica los detalles del token, como las reclamaciones,
            // la fecha de expiración, las credenciales de firma y el emisor y la
            // audiencia del token.

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]

            };

            //Se instancia un manejador de tokens JWT(JwtSecurityTokenHandler)
            //y se utiliza para crear el token JWT utilizando el descriptor de
            //token.

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // devuelve una representación de cadena del token generado
            // utilizando el método WriteToken del manejador de tokens.

            return tokenHandler.WriteToken(token);
        }

    }
}
