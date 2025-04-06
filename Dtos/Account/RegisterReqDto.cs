using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.Account
{
    public class RegisterReqDto
    {

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
