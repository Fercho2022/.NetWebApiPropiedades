using WebApiPropiedades.Models;

namespace WebApiPropiedades.Interface
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync();
        Task<City> GetCityByIdAsync(int Id);
        Task<City> GetCityByNameAsync(string cityName);
        void AddCity(City city);
        void DeleteCity(int id);
        void UpdateCity(City city);
        Task<bool> SaveAsync();
        Task<bool> CityExistsAsync(string cityName);
        Task<bool> CityExistsAsync(int id);


    }
}
