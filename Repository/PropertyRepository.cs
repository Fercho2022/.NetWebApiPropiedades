using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Data;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Repository
{
    public class PropertyRepository : IPropertyRepository
    {

        private readonly DataContext _context;
        public PropertyRepository(DataContext context)
        {
            _context = context;

        }
        public void AddProperty(Property property)
        {
            _context.Properties.Add(property);
        }

        public void DeleteProperty(int id)
        {
            var propertie = _context.Properties.Find(id);
            if (propertie != null)
            {
                _context.Properties.Remove(propertie);
            }
        }

        public async Task<IEnumerable<Property>> GetPropertiesAsync(int sellRent)
        {

            var properties= await _context.Properties
                .Where(p=>p.SellRent == sellRent)
                .Include(p=>p.PropertyType)
                .Include(p=>p.FurnishingType)
                .Include(p=>p.City)
                .Include(p=>p.Photos)
                .ToListAsync();

            
            return properties;
        }

        public async Task<Property> GetPropertyByIdAsync(int id)
        {
           return await _context.Properties.Where(p=>p.Id == id)
                .Include(p=>p.Photos)
                .FirstOrDefaultAsync();
        }

        public async Task<Property> GetPropertyDetailAsync(int id)
        {
            var property=await _context.Properties.Where(p=>p.Id == id)
                .Include(p=>p.PropertyType)
                .Include (p=>p.FurnishingType)
                .Include(p=>p.City)
                .Include(p=>p.Photos)
                .FirstOrDefaultAsync();
            return property;
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync()>0;

        }

        public void UpdateProperty(Property property)
        {
            _context.Properties.Update(property);
        }
    }
}
