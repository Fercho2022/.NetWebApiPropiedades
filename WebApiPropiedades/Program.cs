using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Security.Claims;
using WebApiPropiedades.Data;
using WebApiPropiedades.Extensions;
using WebApiPropiedades.Helpers;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;
using WebApiPropiedades.Repository;
using WebApiPropiedades.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

   
    options.EnableSensitiveDataLogging(); // Habilita logging detallado
    options.LogTo(Console.WriteLine, LogLevel.Information); // Muestra SQL en consola
});





//Configura los servicios de identidad para la aplicación.
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    //Requiere que la contraseña tenga al menos un dígito.
    options.Password.RequireDigit = true;
    //Requiere que la contraseña tenga al menos una letra minuscula.
    options.Password.RequireLowercase = true;
    //Requiere que la contraseña tenga al menos una letra mayúscula.
    options.Password.RequireUppercase = true;
    //Requiere que la contraseña tenga al menos un carácter no
    //alfanumérico (por ejemplo, un símbolo).
    options.Password.RequireNonAlphanumeric = true;
    //Requiere que la contraseña tenga una longitud mínima
    //de 12 caracteres.
    options.Password.RequiredLength = 12;
})
    //Configura Identity para que use Entity Framework Core
    //con el contexto de datos AplicationDbContext para
    //almacenar los datos de identidad.
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders(); // agrega token necesario, incluyendo el que se usa para reset de contraseña.


//Configura los servicios de autenticación para la aplicación.

//Configuración de Identity: Define las reglas de complejidad de las contraseñas
//y configura el uso de Entity Framework Core para almacenar los datos de identidad
//en la base de datos.
builder.Services.AddAuthentication(options =>
{
    //Especifica el esquema de autenticación predeterminado.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Especifica el esquema utilizado para desafíos de autenticación.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema utilizado para acciones de prohibición.
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema de autenticación predeterminado para todas las acciones.
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema utilizado para acciones de inicio de sesión.
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema utilizado para acciones de cierre de sesión.
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

    //Establece los esquemas de autenticación predeterminados para la aplicación
    //y configura la validación de tokens JWT utilizando parámetros definidos en
    //la configuración de la aplicación (appsettings.json o variables de entorno).
}).AddJwtBearer(options => // Configura el manejo de tokens JWT (JSON Web Tokens).

{           //Establece los parámetros para la validación de tokens JWT.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //Habilita la validación del emisor del token.
        ValidateIssuer = true,
        //Especifica el emisor válido del token.
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        //Habilita la validación de la audiencia del token.
        ValidateAudience = true,
        //Especifica la audiencia válida del token.
        ValidAudience = builder.Configuration["JWT:Audience"],
        //Habilita la validación de la clave de firma del emisor
        //del token.
        ValidateIssuerSigningKey = true,
        //Especifica la clave de firma simétrica utilizada para validar la firma del token.
        IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])),
        // Agregar estas dos líneas para el mapeo de claims
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IPropertyTypeRepository, PropertyTypeRepository>();
builder.Services.AddScoped<IFurnishingTypeRepository, FurnishingTypeRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Registrar el servicio transient
builder.Services.AddTransient<IDataSeeder, DataSeeder>();

// Configurar el JSON serializador para evitar ciclos de referencia en las respuestas JSON
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApiPropiedades", Version = "v1" });

    // Configuración del esquema de seguridad JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

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

// Ejecutar la siembra de datos al iniciar
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var seeder = services.GetRequiredService<IDataSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error durante la siembra de datos");
    }

}
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiPropiedades V1");
        c.RoutePrefix = string.Empty; // Esto hará que Swagger sea la página de inicio
    });
}

app.UseCustomExceptionMiddleware();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Usar la política de CORS
app.UseCors("AllowAngularApp");

app.MapControllers();

app.Run();
