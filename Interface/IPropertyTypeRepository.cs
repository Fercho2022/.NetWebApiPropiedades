using WebApiPropiedades.Models;

namespace WebApiPropiedades.Interface
{
    public interface IPropertyTypeRepository
    {
        Task<IEnumerable<PropertyType>> GetAllPropertyTypeAsync();
        Task<PropertyType> GetPropertyTypeByIdAsync(int id);

        Task<PropertyType> GetPropertyTypeByNameAsync(string name);
        Task<bool> PropertyTypeExistAsync(string name);
        Task<bool> SaveAsync();
        void DeletePropertyType(PropertyType propertyType);

        void UpdatePropertyType(PropertyType propertyType);

        void AddPropertyType(PropertyType propertyType);
    }

}
