using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.City
{
    public class CityUpdateDto
    {
        [Required(ErrorMessage = "El ID de la ciudad es obligatorio")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la ciudad es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "El país es obligatorio")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El país debe tener entre 2 y 50 caracteres")]
        public string Country { get; set; }
    }
}
