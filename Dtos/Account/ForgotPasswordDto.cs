using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

    }
}
