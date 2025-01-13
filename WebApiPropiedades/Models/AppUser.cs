using Microsoft.AspNetCore.Identity;

namespace WebApiPropiedades.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; }
    }
}
