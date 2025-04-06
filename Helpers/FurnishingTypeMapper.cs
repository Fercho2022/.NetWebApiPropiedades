using WebApiPropiedades.Dtos.FurnishingType;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Helpers
{
    public class FurnishingTypeMapper
    {
        public static FurnishingTypeDto MapToFurnishingTypeDto(FurnishingType furnishingType)
        {
            return new FurnishingTypeDto
            {
                Id = furnishingType.Id,
                Name = furnishingType.Name
            };
        }

        public static FurnishingTypeDetailDto MapToFurnishingTypeDetailDto(FurnishingType furnishingType)
        {
            return new FurnishingTypeDetailDto
            {
                Id = furnishingType.Id,
                Name = furnishingType.Name,
                LastUpdatedOn = furnishingType.LastUpdatedOn,
                LastUpdatedBy = furnishingType.LastUpdatedBy,
                PropertiesCount = furnishingType.Properties?.Count ?? 0
            };
        }

        public static FurnishingType MapToFurnishingType(FurnishingTypeCreateDto furnishingTypeDto, string userId)
        {
            // Garantizar que userId nunca sea nulo
            if (string.IsNullOrEmpty(userId))
            {
                userId = "sistema";
            }

            return new FurnishingType
            {
                Name = furnishingTypeDto.Name,
                LastUpdatedBy = userId,
                LastUpdatedOn = DateTime.Now
            };
        }

        public static void UpdateFurnishingTypeFromDto(FurnishingType furnishingType, FurnishingTypeUpdateDto furnishingTypeDto, string userId)
        {
            furnishingType.Name = furnishingTypeDto.Name;
            furnishingType.LastUpdatedBy = userId;
            furnishingType.LastUpdatedOn = DateTime.Now;
        }
    }
}

