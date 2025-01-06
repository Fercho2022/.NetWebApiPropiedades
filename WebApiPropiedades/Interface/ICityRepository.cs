using WebApiPropiedades.Models;

namespace WebApiPropiedades.Interface
{
    public interface ICityRepository
    {
        Task<IEnumerable<City>> GetAllCitiesAsync();

        void AddCity(City city);

        void DeleteCity(int cityId);

        Task<bool> SaveAsync();

        Task<City> FindCity(int cityId);
    }
}
