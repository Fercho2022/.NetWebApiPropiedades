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





//Configura los servicios de identidad para la aplicaci�n.
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    //Requiere que la contrase�a tenga al menos un d�gito.
    options.Password.RequireDigit = true;
    //Requiere que la contrase�a tenga al menos una letra minuscula.
    options.Password.RequireLowercase = true;
    //Requiere que la contrase�a tenga al menos una letra may�scula.
    options.Password.RequireUppercase = true;
    //Requiere que la contrase�a tenga al menos un car�cter no
    //alfanum�rico (por ejemplo, un s�mbolo).
    options.Password.RequireNonAlphanumeric = true;
    //Requiere que la contrase�a tenga una longitud m�nima
    //de 12 caracteres.
    options.Password.RequiredLength = 12;
})
    //Configura Identity para que use Entity Framework Core
    //con el contexto de datos AplicationDbContext para
    //almacenar los datos de identidad.
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders(); // agrega token necesario, incluyendo el que se usa para reset de contrase�a.


//Configura los servicios de autenticaci�n para la aplicaci�n.

//Configuraci�n de Identity: Define las reglas de complejidad de las contrase�as
//y configura el uso de Entity Framework Core para almacenar los datos de identidad
//en la base de datos.
builder.Services.AddAuthentication(options =>
{
    //Especifica el esquema de autenticaci�n predeterminado.
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    // Especifica el esquema utilizado para desaf�os de autenticaci�n.
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema utilizado para acciones de prohibici�n.
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema de autenticaci�n predeterminado para todas las acciones.
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema utilizado para acciones de inicio de sesi�n.
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    //Especifica el esquema utilizado para acciones de cierre de sesi�n.
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;

    //Establece los esquemas de autenticaci�n predeterminados para la aplicaci�n
    //y configura la validaci�n de tokens JWT utilizando par�metros definidos en
    //la configuraci�n de la aplicaci�n (appsettings.json o variables de entorno).
}).AddJwtBearer(options => // Configura el manejo de tokens JWT (JSON Web Tokens).

{           //Establece los par�metros para la validaci�n de tokens JWT.
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //Habilita la validaci�n del emisor del token.
        ValidateIssuer = true,
        //Especifica el emisor v�lido del token.
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        //Habilita la validaci�n de la audiencia del token.
        ValidateAudience = true,
        //Especifica la audiencia v�lida del token.
        ValidAudience = builder.Configuration["JWT:Audience"],
        //Habilita la validaci�n de la clave de firma del emisor
        //del token.
        ValidateIssuerSigningKey = true,
        //Especifica la clave de firma sim�trica utilizada para validar la firma del token.
        IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(builder.Configuration["JWT:SigningKey"])),
        // Agregar estas dos l�neas para el mapeo de claims
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

    // Configuraci�n del esquema de seguridad JWT
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
// se utiliza para configurar y registrar el servicio de AutoMapper en una aplicaci�n
// ASP.NET Core. AutoMapper es una biblioteca popular que facilita el
// mapeo de objetos de un tipo a otro, especialmente �til en la
// transformaci�n de datos entre capas de una aplicaci�n (por ejemplo,
// entre la capa de datos y la capa de presentaci�n).

//AddAutoMapper(): m�todo de extensi�n que agrega AutoMapper al contenedor
//de servicios. Al llamarlo, se configura AutoMapper como un servicio
//disponible para ser inyectado en controladores y otros servicios de la
//aplicaci�n.

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // URL de tu app Angular
              .AllowAnyHeader()                   // Permitir cualquier encabezado
              .AllowAnyMethod();                  // Permitir cualquier m�todo HTTP
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
        c.RoutePrefix = string.Empty; // Esto har� que Swagger sea la p�gina de inicio
    });
}

app.UseCustomExceptionMiddleware();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

// Usar la pol�tica de CORS
app.UseCors("AllowAngularApp");

app.MapControllers();

app.Run();
