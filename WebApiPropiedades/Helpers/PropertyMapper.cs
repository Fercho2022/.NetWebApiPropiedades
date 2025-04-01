using WebApiPropiedades.Dtos.City;
using WebApiPropiedades.Dtos.Property;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Helpers
{
    public static class PropertyMapper
    {
        public static PropertyListDto MapToPropertyListDto(Property property)
        {
            return new PropertyListDto
            {

                Id = property.Id,
                SellRent = property.SellRent,
                Name = property.Name,
                PropertyType = property.PropertyType.Name,
                FurnishingType = property.FurnishingType.Name,
                Price = property.Price,
                BHK = property.BHK,
                BuiltArea = property.BuiltArea,
                City = property.City.Name,
                Country = property.City.Country,
                ReadyToMove = property.ReadyToMove,
                EstPossessionOn = property.EstPossessionOn,
                Photo = property.Photos?.FirstOrDefault(p => p.IsPrimary)?.ImageUrl
            };
        }

        public static PropertyDetailDto MapToPropertyDetailDto(Property property)
        {
            return new PropertyDetailDto
            {
                SellRent = property.SellRent,
                Name = property.Name,
                // Uso de operador de navegación segura (?) y operador de coalescencia nula (??)
                PropertyType = property.PropertyType?.Name ?? "No especificado",
                FurnishingType = property.FurnishingType?.Name ?? "No especificado",
                EstPossessionOn = property.EstPossessionOn,
                BuiltArea = property.BuiltArea,
                Price = property.Price,
                BHK = property.BHK,
                CarpetArea = property.CarpetArea,
                Address = property.Address,
                Address2 = property.Address2,
                FloorNo = property.FloorNo,
                TotalFloors = property.TotalFloors,
                MainEntrance = property.MainEntrance,
                Security = property.Security,
                Gated = property.Gated,
                Maintenance = property.Maintenance,
                Age = property.Age,
                Description = property.Description,
                Photos = property.Photos.Select(p => new PhotoDto
                {
                    PublicId = p.PublicId,
                    ImageUrl = p.ImageUrl,
                    IsPrimary = p.IsPrimary
                }).ToList()

            };
        }

        public static Property MapToProperty(PropertyDto propertyDto, string userId = null)
        {
            return new Property
            {
                SellRent = propertyDto.SellRent,
                Name = propertyDto.Name,
                PropertyTypeId = propertyDto.PropertyTypeId,
                FurnishingTypeId = propertyDto.FurnishingTypeId,
                Price = propertyDto.Price,
                BHK = propertyDto.BHK,
                BuiltArea = propertyDto.BuiltArea,
                //CarpetArea = propertyDto.CarpetArea,
                //Address = propertyDto.Address,
                //Address2 = propertyDto.Address2,
                CityId = propertyDto.CityId,
                //FloorNo = propertyDto.FloorNo,
                //TotalFloors = propertyDto.TotalFloors,
                ReadyToMove = propertyDto.ReadyToMove,
                //MainEntrance = propertyDto.MainEntrance,
                //Security = propertyDto.Security,
                //Gated = propertyDto.Gated,
                //Maintenance = propertyDto.Maintenance,
                EstPossessionOn = propertyDto.EstPossessionOn,
                //Age = propertyDto.Age,
                //Description = propertyDto.Description,

                // Campos de BaseEntity
                LastUpdatedBy = userId,
                LastUpdatedOn = DateTime.Now,

                // Campos específicos para Property
                PostedById = userId,
                PostedOn = DateTime.Now
            };
        }

        public static void UpdatePropertyFromDto(Property property, PropertyUpdateDto propertyDto)
        {
            property.SellRent = propertyDto.SellRent;
            property.Name = propertyDto.Name;
            property.PropertyTypeId = propertyDto.PropertyTypeId;
            property.FurnishingTypeId = propertyDto.FurnishingTypeId;
            property.Price = propertyDto.Price;
            property.BHK = propertyDto.BHK;
            property.BuiltArea = propertyDto.BuiltArea;
            property.CarpetArea = propertyDto.CarpetArea;
            property.Address = propertyDto.Address;
            property.Address2 = propertyDto.Address2;
            property.CityId = propertyDto.CityId;
            property.FloorNo = propertyDto.FloorNo;
            property.TotalFloors = propertyDto.TotalFloors;
            property.ReadyToMove = propertyDto.ReadyToMove;
            property.MainEntrance = propertyDto.MainEntrance;
            property.Security = propertyDto.Security;
            property.Gated = propertyDto.Gated;
            property.Maintenance = propertyDto.Maintenance;
            property.EstPossessionOn = propertyDto.EstPossessionOn;
            property.Age = propertyDto.Age;
            property.Description = propertyDto.Description;
        }
    }

}

