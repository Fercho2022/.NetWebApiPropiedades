using System.ComponentModel.DataAnnotations;

namespace WebApiPropiedades.Dtos.Property
{
    public class PropertyCreateDto
    {
        [Required]
        public int SellRent { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int PropertyTypeId { get; set; }

        [Required]
        public int FurnishingTypeId { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public int BHK { get; set; }

        [Required]
        public int BuiltArea { get; set; }


        [Required]
        public int CityId { get; set; }

        public bool ReadyToMove { get; set; }



        public DateTime EstPossessionOn { get; set; }


    }
}
