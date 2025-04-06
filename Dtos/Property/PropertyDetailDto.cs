namespace WebApiPropiedades.Dtos.Property
{
    public class PropertyDetailDto

     {
        public int SellRent { get; set; }
        public string Name { get; set; }
        public string PropertyType { get; set; }
        public string FurnishingType { get; set; }
        public int BuiltArea { get; set; }
        public int CarpetArea { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public int FloorNo { get; set; }

        public int Price { get; set; }

        public int BHK { get; set; }
        public int TotalFloors { get; set; }
        public string MainEntrance { get; set; }
        public int Security { get; set; }
        public bool Gated { get; set; }
        public int Maintenance { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }

        public DateTime EstPossessionOn { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }

    }
}
