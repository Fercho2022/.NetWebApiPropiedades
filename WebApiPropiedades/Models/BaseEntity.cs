namespace WebApiPropiedades.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime LastUpdatedOn { get; set; } = DateTime.Now;
        public string LastUpdatedBy { get; set; }
    }
}
