using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Data;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<ICityRepository, CityRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()):
// se utiliza para configurar y registrar el servicio de AutoMapper en una aplicación
// ASP.NET Core. AutoMapper es una biblioteca popular que facilita el
// mapeo de objetos de un tipo a otro, especialmente útil en la
// transformación de datos entre capas de una aplicación (por ejemplo,
// entre la capa de datos y la capa de presentación).

//AddAutoMapper(): método de extensión que agrega AutoMapper al contenedor
//de servicios. Al llamarlo, se configura AutoMapper como un servicio
//disponible para ser inyectado en controladores y otros servicios de la
//aplicación.

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL de tu app Angular
              .AllowAnyHeader()                   // Permitir cualquier encabezado
              .AllowAnyMethod();                  // Permitir cualquier método HTTP
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Usar la política de CORS
app.UseCors("AllowAngularApp");

app.MapControllers();

app.Run();
