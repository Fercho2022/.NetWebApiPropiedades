namespace WebApiPropiedades.Models
{
    public class PropertyType : BaseEntity
    {
        public string Name { get; set; }

        // Propiedad de navegación
        public ICollection<Property> Properties { get; set; }
    }
}
