using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Claims;
using WebApiPropiedades.Dtos.City;
using WebApiPropiedades.Helpers;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;
using WebApiPropiedades.Repository;

namespace WebApiPropiedades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityRepository _cityRepository;

        private readonly ILogger<CityController> _logger;
        public CityController(ICityRepository cityRepository, ILogger<CityController> logger)
        {
            _cityRepository = cityRepository;

            _logger = logger;

        }

        // GET api/city
        [HttpGet("cities")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCities()
        {
            try
            {
                var cities = await _cityRepository.GetCitiesAsync();

                var cityDto = cities.Select(c => CityMapper.MapToCityDto(c)).ToList();

                return Ok(cityDto);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error retrieving cities");

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving cities");

            }
        }

        // GET: api/city/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id)
        {

            try
            {
                var city = await _cityRepository.GetCityByIdAsync(id);

                if (city == null)
                {
                    return NotFound("City not found");
                }

                var cityDto = CityMapper.MapToCityDto(city);

                return Ok(cityDto);


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving city with ID: {id}");

                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);



            }

        }

        // POST: api/city/post
        [HttpPost("post")]

        [Authorize]// Este atributo asegura que solo usuarios autenticados puedan acceder
        public async Task<IActionResult> AddCity(CityCreateDto cityDto)
        {
            try
            {
                // Validación del modelo

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si la ciudad ya existe

                if (await _cityRepository.CityExistsAsync(cityDto.Name))
                {
                    return BadRequest($"City {cityDto.Name} already exist");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Estándar
    
                // Si userId es nulo, usar un valor predeterminado
                if (string.IsNullOrEmpty(userId))
                {
                    userId = "sistema"; // Asegurarse de que userId nunca sea nulo
                    _logger.LogWarning("No userId found in claims. Using default value.");
                }

                // Mapear y asignar el ID del usuario autenticado
                var city = CityMapper.MapToCity(cityDto, userId);

                // Guardar en la base de datos
                _cityRepository.AddCity(city);

                await _cityRepository.SaveAsync();

                return CreatedAtAction(nameof(AddCity), new { id = city.Id }, CityMapper.MapToCityDetailDto(city));

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error adding city");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error adding city");
            }
        }

        // PUT: api/city/update/5
        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCity(int id, CityUpdateDto cityDto)
        {
            try
            {
                if (id != cityDto.Id)
                {
                    return BadRequest("City Id mismatch");
                }

                var cityFromDb = await _cityRepository.GetCityByIdAsync(cityDto.Id);

                if (cityFromDb == null)
                {
                    return NotFound("City not found");
                }

                // Verificar si el nuevo nombre ya existe para otra ciudad
                var existingCity = await _cityRepository.GetCityByNameAsync(cityDto.Name);
                if (existingCity != null && existingCity.Id != id)
                {
                    return BadRequest($"City {cityDto.Name} already exists");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                CityMapper.UpdateCityFromDto(cityFromDb, cityDto, userId);

                _cityRepository.UpdateCity(cityFromDb);

                await _cityRepository.SaveAsync();

                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating city with ID: {id}");

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating city");

            }
        }

        // Delete api/city/delete/5
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCity(int id)
        {
            try
            {
                var city = await _cityRepository.GetCityByIdAsync(id);

                if (city == null)
                {
                    return NotFound("city not found");
                }

                if (city.Properties != null && city.Properties.Any())
                {

                    return BadRequest("Cannot delete city with associated property");
                }

                _cityRepository.DeleteCity(id);

                await _cityRepository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting city with ID {id}");

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error deleting city");

            }
        }

        [HttpPatch("update/{id}")]
        [Authorize]

        public async Task<IActionResult> UpdateCityPatch(int id, JsonPatchDocument<City> cityToPatch)
        {
            try
            {
                var cityFromDb = await _cityRepository.GetCityByIdAsync(id);

                if (cityFromDb == null)
                {
                    return NotFound("City not found");
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }
                // Actualizar los campos de auditoría
                cityFromDb.LastUpdatedBy = userId;
                cityFromDb.LastUpdatedOn = DateTime.Now;

                // Aplicar el parche a la entidad
                cityToPatch.ApplyTo(cityFromDb, ModelState);

                // Verificar si el modelo es válido después de aplicar el parche
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Guardar los cambios
                _cityRepository.UpdateCity(cityFromDb);
                await _cityRepository.SaveAsync();

                return Ok(CityMapper.MapToCityDetailDto(cityFromDb));
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error to updating city witch Patch for Id: {id}");

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating city");
            }
        }


        [HttpPut("updateCityName/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCityName(int id, CityUpdateDto cityDto)
        {
            try
            {
                if (id != cityDto.Id)
                {
                    return BadRequest("City ID mismatch");
                }
                var cityFromDb = await _cityRepository.GetCityByIdAsync(id);

                if (cityFromDb == null)
                {
                    return NotFound("City not found");
                }

                // Verificar si el nuevo nombre ya existe para otra ciudad
                var existingCity = await _cityRepository.GetCityByNameAsync(cityDto.Name);

                if (existingCity != null && existingCity.Id != id)
                {
                    return BadRequest($"City {cityDto.Name} already exists");
                }

                // Obtener el ID del usuario autenticado
                var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (UserId != null)
                {
                    return Unauthorized("User not authenticated");
                }

                // Actualizar la ciudad con los datos del DTO
                CityMapper.UpdateCityFromDto(cityFromDb, cityDto, UserId);

                // Guardar cambios
                _cityRepository.UpdateCity(cityFromDb);

                await _cityRepository.SaveAsync();

                return Ok(CityMapper.MapToCityDetailDto(cityFromDb));

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error updating city name for ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating city name");
            }
        }



    }

}