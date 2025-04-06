using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using WebApiPropiedades.Dtos.FurnishingType;
using WebApiPropiedades.Helpers;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FurnishingTypeController : ControllerBase
    {
        private readonly IFurnishingTypeRepository _furnishingTypeRepository;
        private readonly ILogger<FurnishingTypeController> _logger;

        public FurnishingTypeController(
            IFurnishingTypeRepository furnishingTypeRepository,
            ILogger<FurnishingTypeController> logger)
        {
            _furnishingTypeRepository = furnishingTypeRepository;
            _logger = logger;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetFurnishingTypes()
        {
            try
            {
                var furnishingTypes = await _furnishingTypeRepository.GetAllFurnishingTypesAsync();
                var furnishingTypesDto = furnishingTypes.Select(ft => FurnishingTypeMapper.MapToFurnishingTypeDto(ft)).ToList();

                return Ok(furnishingTypesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving furnishing types");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving furnishing types");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFurnishingType(int id)
        {
            try
            {
                var furnishingType = await _furnishingTypeRepository.GetFurnishingTypeByIdAsync(id);

                if (furnishingType == null)
                {
                    return NotFound($"Furnishing type with ID: {id} not found");
                }

                var furnishingTypeDto = FurnishingTypeMapper.MapToFurnishingTypeDetailDto(furnishingType);

                return Ok(furnishingTypeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving furnishing type with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error retrieving furnishing type");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddFurnishingType(FurnishingTypeCreateDto furnishingTypeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Verificar si el tipo de amueblado ya existe
                if (await _furnishingTypeRepository.FurnishingTypeExistsAsync(furnishingTypeDto.Name))
                {
                    return BadRequest($"Furnishing type '{furnishingTypeDto.Name}' already exists");
                }

                // Obtener el ID del usuario autenticado
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                var furnishingType = FurnishingTypeMapper.MapToFurnishingType(furnishingTypeDto, userId);

                _furnishingTypeRepository.AddFurnishingType(furnishingType);
                var saved = await _furnishingTypeRepository.SaveAsync();
                _logger.LogInformation($"Resultado de SaveAsync: {saved}");

                if (!saved)
                {
                    return StatusCode(500, "Failed to save the furnishing type to the database");
                }

                return CreatedAtAction(nameof(GetFurnishingType), new { id = furnishingType.Id },
                    FurnishingTypeMapper.MapToFurnishingTypeDto(furnishingType));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding furnishing type");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error adding furnishing type");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFurnishingType(int id, FurnishingTypeUpdateDto furnishingTypeDto)
        {
            try
            {
                if (id != furnishingTypeDto.Id)
                {
                    return BadRequest("Furnishing type ID mismatch");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var furnishingTypeFromDb = await _furnishingTypeRepository.GetFurnishingTypeByIdAsync(id);
                if (furnishingTypeFromDb == null)
                {
                    return NotFound($"Furnishing type with ID: {id} not found");
                }

                // Verificar si el nuevo nombre ya existe para otro tipo de amueblado
                string normalizedNewName = furnishingTypeDto.Name.ToLower();
                string normalizedCurrentName = furnishingTypeFromDb.Name.ToLower();

                if (normalizedNewName != normalizedCurrentName)
                {
                    if (await _furnishingTypeRepository.FurnishingTypeExistsAsync(furnishingTypeDto.Name))
                    {
                        return BadRequest($"Furnishing type name '{furnishingTypeDto.Name}' already exists");
                    }
                }

                // Obtener el ID del usuario autenticado
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                FurnishingTypeMapper.UpdateFurnishingTypeFromDto(furnishingTypeFromDb, furnishingTypeDto, userId);

                _furnishingTypeRepository.UpdateFurnishingType(furnishingTypeFromDb);
                await _furnishingTypeRepository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating furnishing type with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating furnishing type");
            }
        }

        [HttpPatch("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateFurnishingTypePatch(int id, JsonPatchDocument<FurnishingType> furnishingTypeToPatch)
        {
            try
            {
                // Verificar si la ciudad existe
                var furnishingTypeFromDb = await _furnishingTypeRepository.GetFurnishingTypeByIdAsync(id);
                if (furnishingTypeFromDb == null)
                {
                    return NotFound($"Furnishing type with ID: {id} not found");
                }

                // Obtener el ID del usuario autenticado
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User not authenticated");
                }

                // Actualizar los campos de auditoría
                furnishingTypeFromDb.LastUpdatedBy = userId;
                furnishingTypeFromDb.LastUpdatedOn = DateTime.Now;

                // Aplicar el parche a la entidad
                furnishingTypeToPatch.ApplyTo(furnishingTypeFromDb, ModelState);

                // Verificar si el modelo es válido después de aplicar el parche
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Si se cambió el nombre, verificar si ya existe
                var existingTypes = await _furnishingTypeRepository.GetAllFurnishingTypesAsync();
                bool nameExists = existingTypes.Any(ft =>
                    ft.Id != id &&
                    string.Equals(ft.Name, furnishingTypeFromDb.Name, StringComparison.OrdinalIgnoreCase));

                if (nameExists)
                {
                    return BadRequest($"Furnishing type name '{furnishingTypeFromDb.Name}' already exists");
                }

                // Guardar los cambios
                _furnishingTypeRepository.UpdateFurnishingType(furnishingTypeFromDb);
                await _furnishingTypeRepository.SaveAsync();

                return Ok(FurnishingTypeMapper.MapToFurnishingTypeDetailDto(furnishingTypeFromDb));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating furnishing type with PATCH for ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error updating furnishing type");
            }
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteFurnishingType(int id)
        {
            try
            {
                var furnishingType = await _furnishingTypeRepository.GetFurnishingTypeByIdAsync(id);
                if (furnishingType == null)
                {
                    return NotFound($"Furnishing type with ID: {id} not found");
                }

                // Verificar si el tipo de amueblado tiene propiedades asociadas
                if (furnishingType.Properties != null && furnishingType.Properties.Any())
                {
                    return BadRequest("Cannot delete furnishing type with associated properties");
                }

                _furnishingTypeRepository.DeleteFurnishingType(furnishingType);
                await _furnishingTypeRepository.SaveAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting furnishing type with ID: {id}");
                return StatusCode((int)HttpStatusCode.InternalServerError, "Error deleting furnishing type");
            }
        }
    }
}

