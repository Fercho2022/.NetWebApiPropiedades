using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.FurnishingType
{
    public class FurnishingTypeUpdateDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
