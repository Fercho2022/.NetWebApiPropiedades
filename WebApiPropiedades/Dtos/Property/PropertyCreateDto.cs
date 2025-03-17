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

        public int CarpetArea { get; set; }

        [Required]
        public string Address { get; set; }

        public string Address2 { get; set; }

        [Required]
        public int CityId { get; set; }

        public int FloorNo { get; set; }

        public int TotalFloors { get; set; }

        public bool ReadyToMove { get; set; }

        public string MainEntrance { get; set; }

        public int Security { get; set; }

        public bool Gated { get; set; }

        public int Maintenance { get; set; }

        public DateTime EstPossessionOn { get; set; }

        public int Age { get; set; }

        public string Description { get; set; }
    }
}
