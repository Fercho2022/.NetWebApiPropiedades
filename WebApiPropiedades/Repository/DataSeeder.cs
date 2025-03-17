using Microsoft.AspNetCore.Identity;
using WebApiPropiedades.Data;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Repository
{
    public class DataSeeder : IDataSeeder
    {

        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DataSeeder> _logger;

        public DataSeeder(
            DataContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DataSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            try
            {
                await Seed.SeedData(_context, _userManager, _roleManager);
                _logger.LogInformation("Datos sembrados correctamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al sembrar datos");
            }
        }

    }
}