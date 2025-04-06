namespace WebApiPropiedades.Models
{
    public class Property : BaseEntity
    {
        public int SellRent { get; set; }
        public string Name { get; set; } 
        public int Price { get; set; }
        public int BHK { get; set; }
        public int BuiltArea { get; set; }
        public bool ReadyToMove { get; set; }
        public int CarpetArea { get; set; } 
        public string Address { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public int FloorNo { get; set; }
        public int TotalFloors { get; set; }
        public string MainEntrance { get; set; } = string.Empty;
        public int Security { get; set; }
        public bool Gated { get; set; }
        public int Maintenance { get; set; }
        public DateTime EstPossessionOn { get; set; }
        public int Age { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime PostedOn { get; set; } = DateTime.Now;

        // Propiedades de navegación
        public int PropertyTypeId { get; set; }
        public PropertyType PropertyType { get; set; }

        public int FurnishingTypeId { get; set; }
        public FurnishingType FurnishingType { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }

        public string PostedById { get; set; }
        public AppUser PostedBy { get; set; }

        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}
