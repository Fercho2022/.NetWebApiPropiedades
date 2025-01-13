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

        public DbSet<City> Cities { get; set; }

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

        }
    }
}