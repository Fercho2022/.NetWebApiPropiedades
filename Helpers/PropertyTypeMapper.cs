using WebApiPropiedades.Dtos.PropertyType;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Helpers
{
    public static class PropertyTypeMapper
    {
        public static PropertyTypeDto MapToPropertyTypeDto(PropertyType propertyType)
        {
            return new PropertyTypeDto
            {
                Id = propertyType.Id,
                Name = propertyType.Name
            };
        }

        public static PropertyTypeDetailDto MapToPropertyTypeDetailDto(PropertyType propertyType)
        {
            return new PropertyTypeDetailDto
            {
                Id = propertyType.Id,
                Name = propertyType.Name,
                LastUpdatedOn = propertyType.LastUpdatedOn,
                LastUpdatedBy = propertyType.LastUpdatedBy,
                PropertiesCount = propertyType.Properties?.Count ?? 0
            };
        }

        public static PropertyType MapToPropertyType(PropertyTypeCreateDto propertyTypeDto, string userId)
        {
            return new PropertyType
            {
                Name = propertyTypeDto.Name,
                LastUpdatedBy = userId,
                LastUpdatedOn = DateTime.Now
            };
        }

        public static void UpdatePropertyTypeFromDto(PropertyType propertyType, PropertyTypeUpdateDto propertyTypeDto, string userId)
        {
            propertyType.Name = propertyTypeDto.Name;
            propertyType.LastUpdatedBy = userId;
            propertyType.LastUpdatedOn = DateTime.Now;
        }
    }
}

