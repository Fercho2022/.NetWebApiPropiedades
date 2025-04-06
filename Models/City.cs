using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Models
{
    public class City : BaseEntity
    {
        public string Name { get; set; }

        [Required]
        public string Country { get; set; }

        // Propiedad de navegación
        public ICollection<Property> Properties { get; set; }
    }
}

