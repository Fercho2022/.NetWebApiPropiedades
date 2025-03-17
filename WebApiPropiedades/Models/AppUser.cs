using Microsoft.AspNetCore.Identity;

namespace WebApiPropiedades.Models
{
   
 public class AppUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; }

        // Propiedad de navegación para las propiedades publicadas por este usuario
        public ICollection<Property> PropertiesListed { get; set; } = new List<Property>();
    }
}
