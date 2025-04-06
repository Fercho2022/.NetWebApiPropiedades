using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using WebApiPropiedades.Data;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Repository
{
    public class CityRepository : ICityRepository
    {
        private readonly DataContext _context;
        public CityRepository(DataContext context)
        {
            _context = context;

        }
        public void AddCity(City city)
        {
            _context.Cities.Add(city);
        }

        public void DeleteCity(int cityId)
        {
            var city = _context.Cities.Find(cityId);

            if (city != null)
            {
                _context.Cities.Remove(city);
            }


        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Cities.Include(p => p.Properties).ToListAsync();
        }

        public async Task<City> GetCityByIdAsync(int Id)
        {
            return await _context.Cities.Include(p=>p.Properties).FirstOrDefaultAsync(c=>c.Id==Id);
        }

        public async Task<City> GetCityByNameAsync(string cityName)
        {
            return await _context.Cities.Include(p => p.Properties).FirstOrDefaultAsync(p=>p.Name==cityName);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void UpdateCity(City city)
        {
           _context.Cities.Update(city);
        }

        public async Task<bool> CityExistsAsync(string cityName)
        {
            return await _context.Cities.AnyAsync(p => p.Name==cityName);
        }

        public async Task<bool> CityExistsAsync(int id)
        {
           return await _context.Cities.AnyAsync(c=>c.Id == id);
        }

        
    }
}
