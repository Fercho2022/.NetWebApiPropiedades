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
            var city=_context.Cities.Find(cityId);

            _context.Cities.Remove(city);
        }

        public async Task<IEnumerable<City>> GetAllCitiesAsync()
        {
            return await _context.Cities.ToListAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync()>0;
        }
    }
}
