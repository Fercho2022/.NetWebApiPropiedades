using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Data;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Repository
{
    public class FurnishingTypeRepository : IFurnishingTypeRepository
    {
        private readonly DataContext _context;

        public FurnishingTypeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FurnishingType>> GetAllFurnishingTypesAsync()
        {
            return await _context.FurnishingTypes
                .Include(ft => ft.Properties)
                .ToListAsync();
        }

        public async Task<FurnishingType?> GetFurnishingTypeByIdAsync(int id)
        {
            return await _context.FurnishingTypes
                         .Include(ft => ft.Properties)
                .FirstOrDefaultAsync(ft => ft.Id == id);
        }

        public async Task<FurnishingType?> GetFurnishingTypeByNameAsync(string name)
        {
            return await _context.FurnishingTypes
                .Include(ft => ft.Properties)
                .FirstOrDefaultAsync(ft => ft.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> FurnishingTypeExistsAsync(string name)
        {
            string normalizedName = NormalizeString(name);
            return await _context.FurnishingTypes
                .AnyAsync(ft => NormalizeString(ft.Name) == normalizedName);
        }

        public async Task<bool> FurnishingTypeExistsAsync(int id)
        {
            return await _context.FurnishingTypes
                .AnyAsync(ft => ft.Id == id);
        }

        public void AddFurnishingType(FurnishingType furnishingType)
        {
            _context.FurnishingTypes.Add(furnishingType);
        }

        public void UpdateFurnishingType(FurnishingType furnishingType)
        {
            _context.FurnishingTypes.Update(furnishingType);
        }

        public void DeleteFurnishingType(FurnishingType furnishingType)
        {
            _context.FurnishingTypes.Remove(furnishingType);
        }

        public async Task<bool> SaveAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // Método auxiliar para normalizar cadenas (eliminar acentos y convertir a minúsculas)
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

            return normalized;
        }
    }
    
    
}
