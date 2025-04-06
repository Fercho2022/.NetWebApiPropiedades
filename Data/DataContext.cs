using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WebApiPropiedades.Data
{
    public class DataContext : IdentityDbContext<AppUser>
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<FurnishingType> FurnishingTypes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Photo> Photos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Llama al método OnModelCreating de la clase base para aplicar las 
            //configuraciones predeterminadas.

            base.OnModelCreating(builder);

            //Se crea una lista de objetos IdentityRole que representan los roles 
            //en la aplicación. Estos roles son "Admin" y "User".

            List<IdentityRole> roles = new List<IdentityRole>
                {
                    new IdentityRole
                    {
                        Id = "1",  // Agregar ID único
                        Name="Admin",
                        NormalizedName="ADMIN"
                    },
                    new IdentityRole
                    {
                        Id = "2",  // Agregar ID único
                        Name="User",
                        NormalizedName="USER"
                    },
            };

            //Utiliza el objeto builder para configurar los datos iniciales del modelo
            //para la entidad IdentityRole.En este caso, se insertan los roles 
            //predefinidos en la base de datos cuando se aplica la migración inicial.

            builder.Entity<IdentityRole>().HasData(roles);

            // Relaciones de Property
            builder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Property>()
                .HasOne(p => p.FurnishingType)
                .WithMany(ft => ft.Properties)
                .HasForeignKey(p => p.FurnishingTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Property>()
                .HasOne(p => p.City)
                .WithMany(c => c.Properties)
                .HasForeignKey(p => p.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Property>()
                .HasOne(p => p.PostedBy)
                .WithMany(u => u.PropertiesListed)
                .HasForeignKey(p => p.PostedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación de Photo con Property
            builder.Entity<Photo>()
                .HasOne(ph => ph.Property)
                .WithMany(p => p.Photos)
                .HasForeignKey(ph => ph.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuraciones adicionales
            builder.Entity<City>()
                .Property(c => c.Name)
                .IsRequired();

            builder.Entity<FurnishingType>()
                .Property(f => f.Name)
                .IsRequired();

            builder.Entity<PropertyType>()
                .Property(pt => pt.Name)
                .IsRequired();

        }
    }
}