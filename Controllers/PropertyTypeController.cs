using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using WebApiPropiedades.Dtos.PropertyType;
using WebApiPropiedades.Helpers;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Repository;

namespace WebApiPropiedades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyTypeController : ControllerBase
    {

        private readonly IPropertyTypeRepository _propertyTypeRepository;

        private readonly ILogger<PropertyTypeController> _logger;
        public PropertyTypeController(IPropertyTypeRepository propertyTypeRepository, ILogger<PropertyTypeController> logger)
        {
            _propertyTypeRepository = propertyTypeRepository;

            _logger = logger;

        }

        [HttpGet("list")]

        public async Task<IActionResult> GetPropertyTypes()
        {

            try
            {
                var propertyTypes = await _propertyTypeRepository.GetAllPropertyTypeAsync();

                var PropertyTypesDto = propertyTypes.Select(pt => PropertyTypeMapper.MapToPropertyTypeDto(pt));

                return Ok(PropertyTypesDto);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error retrieving property types");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving property types");

            }


        }

        [HttpGet("propertyType/{id}")]
        [Authorize]

        public async Task<IActionResult> GetPropertyType(int id)
        {
            try
            {
                var propertyType = await _propertyTypeRepository.GetPropertyTypeByIdAsync(id);

                if (propertyType == null)
                {
                    return NotFound($"PropertyType with Id: {id} not found");
                }

                var propertyTypeDto = PropertyTypeMapper.MapToPropertyTypeDto(propertyType);

                return Ok(propertyTypeDto);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, $"Error retrieving property type with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving property type");
            }
        }

        [HttpPost]
        [Authorize]

        public async Task<IActionResult> AddPropertyType(PropertyTypeCreateDto propertyTypeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                // Verificar si el tipo de propiedad ya existe
                if (await _propertyTypeRepository.PropertyTypeExistAsync(propertyTypeDto.Name)){
                   
                    return BadRequest($"PropertyType '{propertyTypeDto.Name}' already exists ");
                }

                // Obtener el ID del usuario autenticado
                var userId=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId)){
                    return Unauthorized("User not authenticated");
                }

                var propertyType=PropertyTypeMapper.MapToPropertyType(propertyTypeDto, userId);

                _propertyTypeRepository.AddPropertyType(propertyType);

                var saved = await _propertyTypeRepository.SaveAsync();

                _logger.LogInformation($"Resultado de SaveAsync: {saved}");

                return CreatedAtAction(nameof(GetPropertyType), new { id = propertyType.Id },
                    PropertyTypeMapper.MapToPropertyTypeDto(propertyType));

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error adding property type");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error adding property type");
            }

        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePropertyType(int id, PropertyTypeUpdateDto propertyTypeDto)
        {
            try
            {
                if (id != propertyTypeDto.Id)
                {
                    return BadRequest("Property type ID mismatch");
                }

                if(!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var propertyTypeFromDb=await _propertyTypeRepository.GetPropertyTypeByIdAsync(propertyTypeDto.Id);

                if(propertyTypeFromDb == null)
                {
                    return NotFound($"Property type with {id} not found");
                }

                // Normalizar el nombre antes de comparar
                string normalizedNewName = NormalizeString(propertyTypeDto.Name);
                string normalizedCurrentName = NormalizeString(propertyTypeFromDb.Name);

                // Verificar si el nuevo nombre normalizado ya existe
                if (normalizedNewName != normalizedCurrentName)
                {
                    // Verificar si existe otro tipo con el mismo nombre normalizado
                    var existingTypes = await _propertyTypeRepository.GetAllPropertyTypeAsync();
                    bool nameExists = existingTypes.Any(pt =>
                        pt.Id != id &&
                        NormalizeString(pt.Name) == normalizedNewName);

                    if (nameExists)
                    {
                        return BadRequest($"Property type name '{propertyTypeDto.Name}' already exists");
                    }
                }
                // Obtener el ID del usuario autenticado

                var userId=User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                PropertyTypeMapper.UpdatePropertyTypeFromDto(propertyTypeFromDb, propertyTypeDto, userId);

                await _propertyTypeRepository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating property type with Id: {id}");

                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating property type");

            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePropertyType(int id)
        {
            try
            {
                var propertyType = await _propertyTypeRepository.GetPropertyTypeByIdAsync(id);
                if (propertyType == null)
                {
                    return NotFound($"Property type with ID: {id} not found");
                }

                // Verificar si el tipo de propiedad tiene propiedades asociadas
                if (propertyType.Properties != null && propertyType.Properties.Any())
                {
                    return BadRequest("Cannot delete property type with associated properties");
                }

                _propertyTypeRepository.DeletePropertyType(propertyType);
                await _propertyTypeRepository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting property type with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error deleting property type");
            }
        }

        // Método auxiliar para normalizar cadenas
        private string NormalizeString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Convertir a minúsculas
            string normalized = input.ToLowerInvariant();

            // Normalizar caracteres (remover acentos)
            normalized = new string(
                normalized.Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) !=
                       System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());

            // Opcionalmente, también podrías remover espacios en blanco
            normalized = normalized.Replace(" ", "");

            return normalized;
        }


    }

   
}
