using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Data;
using WebApiPropiedades.Dtos;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        

        private readonly ICityRepository cityRepository;

        private readonly IMapper mapper;

        public CityController(ICityRepository cityRepository, IMapper mapper)
        {
            this.cityRepository = cityRepository;

            this.mapper = mapper;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetCities()
        {
           var cities = await this.cityRepository.GetAllCitiesAsync();

            var citiesDto=mapper.Map<IEnumerable<CityDto>>(cities);

                       
            return Ok(citiesDto);
        }

        // Post api/city/add/{cityName}
        [HttpPost("add")]
        public async Task<IActionResult> AddCity(string cityName)
        {
           City city= new City();
            city.Name = cityName;
            this.cityRepository.AddCity(city);
            await this.cityRepository.SaveAsync();
            return Ok(city);
        }

        // Post api/city/post  --Post the data in JSON Format
        [HttpPost("post")]
        public async Task<IActionResult> AddCity(CityDto cityDto)
        {

            City city=mapper.Map<City>(cityDto);

            city.LastUpdatedBy = 1;

            city.LastUpdatedOn= DateTime.Now;
            
            this.cityRepository.AddCity(city);

            await this.cityRepository.SaveAsync();

            return StatusCode(201);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            this.cityRepository.DeleteCity(id);

            await this.cityRepository.SaveAsync();

            return Ok(id);

                   

           
        }
    }
}
