using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.PropertyType
{

    // DTO para crear un nuevo tipo de propiedad
    public class PropertyTypeCreateDto
    {

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
