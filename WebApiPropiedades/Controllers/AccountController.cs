using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiPropiedades.Dtos.Account;
using WebApiPropiedades.Interface;
using WebApiPropiedades.Models;

namespace WebApiPropiedades.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {


        //Declara tres variables privadas de solo lectura _userManager del tipo 
        //UserManager<AppUser>, _tokenService del tipo ITokenService y _signInManager
        //del tipo SignInManager<AppUser>. UserManager, ITokenService y SignInManager
        //son clases proporcionadas por ASP.NET Core Identity para manejar operaciones
        //relacionadas con la gestion de usuarios y tokens, 


        private readonly UserManager<AppUser> _userManager;

        private readonly ITokenService _tokenService;

        private readonly SignInManager<AppUser> _signInManager;

        //Este es el constructor de la clase AccountController, que recibe los 
        //objetos UserManager<AppUser>, ITokenService y SignInManager<AppUser> a través de la inyección de dependencias.
        //Esta es una práctica común en ASP.NET Core para administrar las 
        //dependencias de los controladores.
        public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginReqDto loginReqDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Si solo necesitas el usuario por email, usa FindByEmailAsync
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginReqDto.UserName.ToLower());


            if (user == null)
            {
                return Unauthorized("email inválido!");

            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginReqDto.Password, false);

            if (!result.Succeeded)
            {
                return Unauthorized("email no encontrado y/o password incorrecto");
            }

            return Ok(
                new NewUserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Token = _tokenService.CreateToken(user)
                }
                );
        }

        //Este atributo indica que el método Register responde a las solicitudes
        //HTTP POST dirigidas a la ruta "api/Account/register".

        [HttpPost("register")]

        //Este es el método que maneja las solicitudes de registro de nuevos 
        //usuarios.Toma un objeto RegisterDTO del cuerpo de la solicitud HTTP 
        //y devuelve un objeto IActionResult, que es la interfaz base para todo 
        //los resultados de acción.
        public async Task<IActionResult> Register([FromBody] RegisterReqDto registerReqDto)
        {
            //código para procesar la solicitud de registro de usuario. 
            //asigna el rol "User" al usuario creado y devuelve respuestas HTTP 
            //correspondientes según el resultado de la operación. En caso de 
            //error, devuelve un código de estado 500 junto con detalles del error.
            try
            {
                // Verifica la validez del modelo

                if (!ModelState.IsValid) return BadRequest(ModelState);

                // crea un nuevo usuario utilizando UserManager,

                var appUser = new AppUser
                {
                    UserName = registerReqDto.UserName,
                    Email = registerReqDto.Email,
                    CreatedAt = DateTime.UtcNow,  // Agregamos esta línea
                                                  // Otros campos que necesites inicializar
                };

                //CreateAsync es un metodo que devuelve un objeto "IdentityResult"
                var createUser = await _userManager.CreateAsync(appUser, registerReqDto.Password);

                if (createUser.Succeeded)
                {

                    //AddToRoleAsync: Este es un método de UserManager que
                    //se utiliza para agregar un usuario a un rol específico
                    //de forma asincrónica. Toma dos parámetros: el objeto
                    //AppUser que representa el usuario al que se va a agregar
                    //el rol y el nombre del rol al que se va a agregar el
                    //usuario. la variable roleResult, que probablemente sea
                    //del tipo IdentityResult, similar al resultado devuelto
                    //por el método CreateAsync. Este objeto IdentityResult
                    //indica si la operación de agregar el usuario al rol se
                    //realizó correctamente o si hubo algún error.

                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                    if (roleResult.Succeeded)
                    {
                        return Ok(

                           new NewUserDto
                           {
                               UserName = appUser.UserName,
                               Email = appUser.Email,
                               Token = _tokenService.CreateToken(appUser)
                           }
                            );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }

                }
                else
                {
                    return StatusCode(500, createUser.Errors);
                }
            }
            catch (Exception ex)
            {

                return StatusCode(500, ex);
            }

        }
    }
}
