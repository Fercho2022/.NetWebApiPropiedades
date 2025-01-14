using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Data;
using WebApiPropiedades.Dtos.City;
using WebApiPropiedades.Exceptions;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;
using WebApiPropiedades.Repository;

namespace WebApiPropiedades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        //[AllowAnonymous]
        public async Task<IActionResult> GetCities()
        {

            

            var cities = await this.cityRepository.GetAllCitiesAsync();

            var citiesDto = mapper.Map<IEnumerable<CityDto>>(cities);


            return Ok(citiesDto);
        }

        // Post api/city/add/{cityName}
        [HttpPost("add")]
        public async Task<IActionResult> AddCity(string cityName)
        {
            City city = new City();
            city.Name = cityName;
            this.cityRepository.AddCity(city);
            await this.cityRepository.SaveAsync();
            return Ok(city);
        }

        // Post api/city/post  --Post the data in JSON Format
        [HttpPost("post")]
        public async Task<IActionResult> AddCity([FromBody] CityDto cityDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            City city = mapper.Map<City>(cityDto);

            city.LastUpdatedBy = 1;

            city.LastUpdatedOn = DateTime.Now;

            this.cityRepository.AddCity(city);

            await this.cityRepository.SaveAsync();

            return StatusCode(201);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityDto cityDto)
        {

            if (id != cityDto.Id)
            {
                return BadRequest("Actualización no permitida");


            }

            var cityFromDb = await this.cityRepository.FindCity(id);

            if (cityFromDb == null)
            {
                return BadRequest("Actualización no permitida, no se encuentra Id");
            }

            cityFromDb.LastUpdatedBy = 1;

            cityFromDb.LastUpdatedOn = DateTime.Now;

            mapper.Map(cityDto, cityFromDb);

            throw new Exception("algo ocurrió");

            await this.cityRepository.SaveAsync();

            return StatusCode(200);
        }



        [HttpPut("updateCityName/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityUpdateDto cityUpdateDto)
        {

            var cityFromDb = await this.cityRepository.FindCity(id);

            cityFromDb.LastUpdatedBy = 1;

            cityFromDb.LastUpdatedOn = DateTime.Now;

            mapper.Map(cityUpdateDto, cityFromDb);

            await this.cityRepository.SaveAsync();

            return StatusCode(200);

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
