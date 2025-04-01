using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.PropertyType
{

    // DTO para actualizar un tipo de propiedad existente
    public class PropertyTypeUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
