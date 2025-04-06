using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Models
{
    public class Photo : BaseEntity
    {
        [Required]
        public string PublicId { get; set; }

        [Required]
        public string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }

        // Propiedad de navegación
        public int PropertyId { get; set; }
        public Property Property { get; set; }
    }
}
