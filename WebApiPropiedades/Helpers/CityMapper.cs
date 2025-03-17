using Org.BouncyCastle.Bcpg;
using WebApiPropiedades.Dtos.City;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Helpers
{
    public static class CityMapper
    {

        public static CityDto MapToCityDto(City city)
        {
            return new CityDto
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
            };
        }

        public static City MapToCity(CityCreateDto cityDto, string userId)
        {

            // Garantizar que userId nunca sea nulo
            if (string.IsNullOrEmpty(userId))
            {
                userId = "sistema"; // Valor por defecto
            }

            return new City
            {
                Name = cityDto.Name,
                Country = cityDto.Country,
                LastUpdatedBy = userId,
                LastUpdatedOn = DateTime.Now
            };
        }

        public static CityDetailDto MapToCityDetailDto(City city)
        {

            return new CityDetailDto
            {
                Id = city.Id,
                Name = city.Name,
                Country = city.Country,
                LastUpdatedOn = city.LastUpdatedOn,
                LastUpdatedBy = city.LastUpdatedBy
            };
        }

        public static void UpdateCityFromDto(City city, CityUpdateDto cityDto, string userId)
        {


            city.Name = cityDto.Name;
            city.Country = cityDto.Country;
            city.LastUpdatedBy = userId;
            city.LastUpdatedOn = DateTime.Now;


        }


    }
}
