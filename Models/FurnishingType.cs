using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Models
{
    public class FurnishingType : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        // Propiedad de navegación
        public ICollection<Property> Properties { get; set; }
    }
}
