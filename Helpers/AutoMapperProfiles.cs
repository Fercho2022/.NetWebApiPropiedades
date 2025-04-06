using AutoMapper;
using WebApiPropiedades.Dtos.City;
using WebApiPropiedades.Dtos.Property;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<City, CityDto>().ReverseMap();
            CreateMap<City, CityUpdateDto>().ReverseMap();
           


           

        }
    }
}
