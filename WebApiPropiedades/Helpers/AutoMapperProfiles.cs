using AutoMapper;
using WebApiPropiedades.Dtos;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<City, CityDto>().ReverseMap();
           
        }
    }
}
