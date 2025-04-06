using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Data
{
    public static class Seed
    {


        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            try
            {
                // Crear usuario admin
                var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
                if (adminUser == null)
                {
                    adminUser = new AppUser
                    {
                        UserName = "admin@gmail.com",
                        NormalizedUserName = "ADMIN@GMAIL.COM",
                        Email = "admin@gmail.com",
                        NormalizedEmail = "ADMIN@GMAIL.COM",
                        EmailConfirmed = true,
                        CreatedAt = DateTime.Now
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Admin");
                    }
                }

                // Sembrar tipos de propiedades
                if (!await context.PropertyTypes.AnyAsync())
                {
                    var propertyTypes = new[]
                    {
                new PropertyType { Name = "Casa", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" },
                new PropertyType { Name = "Apartamento", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" },
                new PropertyType { Name = "Dúplex", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" }
            };

                    await context.PropertyTypes.AddRangeAsync(propertyTypes);
                    await context.SaveChangesAsync();
                }

                // Sembrar tipos de amueblado
                if (!await context.FurnishingTypes.AnyAsync())
                {
                    var furnishingTypes = new[]
                    {
                new FurnishingType { Name = "Completo", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" },
                new FurnishingType { Name = "Semi amueblado", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" },
                new FurnishingType { Name = "Sin amueblar", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" }
            };

                    await context.FurnishingTypes.AddRangeAsync(furnishingTypes);
                    await context.SaveChangesAsync();
                }

                // Sembrar ciudades
                if (!await context.Cities.AnyAsync())
                {
                    var cities = new[]
                    {
                new City { Name = "Buenos Aires", Country = "Argentina", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" },
                new City { Name = "Córdoba", Country = "Argentina", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" },
                new City { Name = "Rosario", Country = "Argentina", LastUpdatedOn = DateTime.Now, LastUpdatedBy = "system" }
            };

                    await context.Cities.AddRangeAsync(cities);
                    await context.SaveChangesAsync();
                }

                // Solo intentar crear propiedades si no existen
                if (!await context.Properties.AnyAsync())
                {
                    try
                    {
                        // Obtener referencias
                        var adminId = adminUser.Id;
                        var casaType = await context.PropertyTypes.FirstOrDefaultAsync(pt => pt.Name == "Casa");
                        var completoType = await context.FurnishingTypes.FirstOrDefaultAsync(ft => ft.Name == "Completo");
                        var buenosAires = await context.Cities.FirstOrDefaultAsync(c => c.Name == "Buenos Aires");

                        if (casaType != null && completoType != null && buenosAires != null)
                        {
                            // Usar transacción y rastreo limpio
                            context.ChangeTracker.Clear();
                            using var transaction = await context.Database.BeginTransactionAsync();

                            try
                            {
                                var property = new Property
                                {
                                    SellRent = 1, // Venta
                                    Name = "Casa Moderna en BA",
                                    PropertyTypeId = casaType.Id,
                                    FurnishingTypeId = completoType.Id,
                                    CityId = buenosAires.Id,
                                    Price = 15000,
                                    BHK = 3,
                                    BuiltArea = 1800,
                                    CarpetArea = 1500,
                                    Address = "Av. Libertador 1500",
                                    Address2 = "Piso 5",
                                    FloorNo = 5,
                                    TotalFloors = 10,
                                    ReadyToMove = true,
                                    MainEntrance = "Este",
                                    Security = 1,
                                    Gated = true,
                                    Maintenance = 500,
                                    EstPossessionOn = DateTime.Now,
                                    Age = 2,
                                    Description = "Hermosa casa moderna totalmente amueblada con excelentes vistas",
                                    PostedOn = DateTime.Now,
                                    PostedById = adminId,
                                    LastUpdatedOn = DateTime.Now,
                                    LastUpdatedBy = adminId
                                };

                                context.Properties.Add(property);
                                await context.SaveChangesAsync();
                                await transaction.CommitAsync();
                            }
                            catch
                            {
                                await transaction.RollbackAsync();
                                throw;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al sembrar propiedades: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SeedData: {ex.Message}");
            }
        }
    }
}
