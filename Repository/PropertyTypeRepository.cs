using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Data;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Repository
{
    public class PropertyTypeRepository : IPropertyTypeRepository
    {
        private readonly DataContext _context;
        public PropertyTypeRepository(DataContext context)
        {

            _context = context;

        }

        public void AddPropertyType(PropertyType propertyType)
        {
            _context.PropertyTypes.Add(propertyType);
        }

        public void DeletePropertyType(PropertyType propertyType)
        {
            _context.PropertyTypes.Remove(propertyType);
        }

        public async Task<IEnumerable<PropertyType>> GetAllPropertyTypeAsync()
        {
            return await _context.PropertyTypes
                 .Include(pt => pt.Properties)
                 .ToListAsync();
        }

        public async Task<PropertyType> GetPropertyTypeByIdAsync(int id)
        {
            return await _context.PropertyTypes
                .Include(pt => pt.Properties)
                .FirstOrDefaultAsync(pt => pt.Id == id);
        }

        public async Task<PropertyType> GetPropertyTypeByNameAsync(string name)
        {
            return await _context.PropertyTypes
                .Include(pt => pt.Properties)
                .FirstOrDefaultAsync(pt => pt.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> PropertyTypeExistAsync(string name)
        {
         return await _context.PropertyTypes.AnyAsync(pt => pt.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> SaveAsync()
        {
          return await _context.SaveChangesAsync()>0;
        }

        public void UpdatePropertyType(PropertyType propertyType)
        {
           _context.PropertyTypes.Update(propertyType);
        }
    }
}
