namespace WebApiPropiedades.Dtos.PropertyType
{

    // DTO con información detallada
    public class PropertyTypeDetailDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public int PropertiesCount { get; set; }
    }
}
