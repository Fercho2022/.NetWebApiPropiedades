

using WebApiPropiedades.Models;

namespace WebApiPropiedades.Interface
{
    public interface IPropertyRepository
    {
        Task<IEnumerable<Property>> GetPropertiesAsync(int sellRent);

        Task<Property> GetPropertyDetailAsync(int id);
        void AddProperty(Property property);
        void DeleteProperty(int id);

        Task<bool> SaveAsync();

        void UpdateProperty(Property property);

        Task<Property> GetPropertyByIdAsync(int id);

       
    }
}
