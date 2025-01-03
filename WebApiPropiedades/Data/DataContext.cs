using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Data
{
    public class DataContext:DbContext
    {

        public DataContext(DbContextOptions<DataContext> options):base(options)
        {


           
        }

        public DbSet<City> Cities { get; set; }
}
}
