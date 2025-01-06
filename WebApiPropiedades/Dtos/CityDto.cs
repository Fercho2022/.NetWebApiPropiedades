using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos
{
    public class CityDto
    {

        public int Id { get; set; }

        [Required]
        
        public string Name { get; set; }

        [Required(ErrorMessage = "Country is mandatory field")]

        public string Country { get; set; }

        
    }
}
