using WebApiPropiedades.Models;

namespace WebApiPropiedades.Interface
{
    public interface IFurnishingTypeRepository
    {
        Task<IEnumerable<FurnishingType>> GetAllFurnishingTypesAsync();
        Task<FurnishingType?> GetFurnishingTypeByIdAsync(int id);
        Task<FurnishingType?> GetFurnishingTypeByNameAsync(string name);
        Task<bool> FurnishingTypeExistsAsync(string name);
        Task<bool> FurnishingTypeExistsAsync(int id);
        void AddFurnishingType(FurnishingType furnishingType);
        void UpdateFurnishingType(FurnishingType furnishingType);
        void DeleteFurnishingType(FurnishingType furnishingType);
        Task<bool> SaveAsync();
    }
}
