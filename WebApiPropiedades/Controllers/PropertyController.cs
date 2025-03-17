
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using WebApiPropiedades.Data;
using WebApiPropiedades.Dtos.Property;
using WebApiPropiedades.Helpers;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;





namespace WebApiPropiedades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {

        private readonly IPropertyRepository _propertyRepository;

        private readonly ILogger<PropertyController> _logger;

        private readonly UserManager<AppUser> _userManager;
       
        private readonly DataContext _context;
        public PropertyController(IPropertyRepository propertyRepository, ILogger<PropertyController> logger, UserManager<AppUser> userManager,
    DataContext context)
        {

            _propertyRepository = propertyRepository;

            _logger = logger;

            _userManager = userManager;

            _context = context;

        }

        // GET: api/property/list/1 (1 for Sell, 2 for Rent)
        [HttpGet("list/{sellRent}")]
        [Authorize]
        public async Task<IActionResult> GetProperties(int sellRent)
        {

            try
            {
                var properties = await _propertyRepository.GetPropertiesAsync(sellRent);
               
                var propertyListDto = properties.Select(p => PropertyMapper.MapToPropertyListDto(p)).ToList();
               
                return Ok(propertyListDto);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving properties");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving properties");
            }
        }

        // GET: api/property/detail/5
        [HttpGet("detail/{id}")]
        //[Authorize]
        public async Task<IActionResult> GetPropertyDetail(int id)
        {
            try
            {
                var property = await _propertyRepository.GetPropertyDetailAsync(id);
                if (property == null)
                {
                    return NotFound("Property not found");
                }

                var propertyDto = PropertyMapper.MapToPropertyDetailDto(property);
                return Ok(propertyDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving property with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving property details");
            }
        }

        // POST: api/property
        [HttpPost]
        [Authorize] // Asegúrate de que este atributo esté presente
        public async Task<IActionResult> AddProperty(PropertyCreateDto propertyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Intenta obtener el ID del usuario autenticado
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


                // Si no hay usuario autenticado, busca un usuario existente en la BD
                if (string.IsNullOrEmpty(userId))
                {
                    // Opción 1: Usa el UserManager para obtener un usuario específico
                    var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                    
                    var systemUser = await userManager.FindByNameAsync("admin"); // o cualquier usuario que sepas que existe

                    if (systemUser != null)
                    {
                        userId = systemUser.Id;
                    }
                    else
                    {
                        // Opción 2: Consulta directamente la base de datos
                        var dbContext = HttpContext.RequestServices.GetRequiredService<DataContext>();
                       
                        var anyUser = await dbContext.Users.FirstOrDefaultAsync();

                        if (anyUser != null)
                        {
                            userId = anyUser.Id;
                        }
                        else
                        {
                            // No hay usuarios en la BD
                            return StatusCode(500, "No hay usuarios disponibles para asignar como propietario de la propiedad");
                        }
                    }
                }

                var property = PropertyMapper.MapToProperty(propertyDto, userId);

                // Set the current user as the property poster (this would typically come from authentication)
                // property.PostedById = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                _propertyRepository.AddProperty(property);
                await _propertyRepository.SaveAsync();

                return CreatedAtAction("GetPropertyDetail", new { id = property.Id },
                    PropertyMapper.MapToPropertyDetailDto(property));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding property");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error adding property");
            }
        }

        // PUT: api/property/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateProperty(int id, PropertyUpdateDto propertyDto)
        {
            try
            {
                if (id != propertyDto.Id)
                {
                    return BadRequest("Property ID mismatch");
                }

                var propertyFromDb = await _propertyRepository.GetPropertyDetailAsync(id);
                if (propertyFromDb == null)
                {
                    return NotFound("Property not found");
                }

                // Check if the user is authorized to update this property
                // if (propertyFromDb.PostedById != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                // {
                //     return Unauthorized("You are not authorized to update this property");
                // }

                PropertyMapper.UpdatePropertyFromDto(propertyFromDb, propertyDto);

                _propertyRepository.UpdateProperty(propertyFromDb);

                await _propertyRepository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating property with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating property");
            }
        }

        // DELETE: api/property/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            try
            {
                var property = await _propertyRepository.GetPropertyDetailAsync(id);
                if (property == null)
                {
                    return NotFound("Property not found");
                }

                // Check if the user is authorized to delete this property
                // if (property.PostedById != User.FindFirst(ClaimTypes.NameIdentifier)?.Value)
                // {
                //     return Unauthorized("You are not authorized to delete this property");
                // }

                _propertyRepository.DeleteProperty(id);
                await _propertyRepository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting property with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error deleting property");
            }
        }

    }




   }

