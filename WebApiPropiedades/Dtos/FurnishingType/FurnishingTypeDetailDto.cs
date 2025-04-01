namespace WebApiPropiedades.Dtos.FurnishingType
{
    public class FurnishingTypeDetailDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public int PropertiesCount { get; set; }
    }
}