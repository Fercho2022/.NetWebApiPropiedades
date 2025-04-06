using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.FurnishingType
{
    public class FurnishingTypeCreateDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
