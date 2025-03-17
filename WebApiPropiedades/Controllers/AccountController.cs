using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
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

        private readonly IEmailService _emailService;

        //Este es el constructor de la clase AccountController, que recibe los 
        //objetos UserManager<AppUser>, ITokenService y SignInManager<AppUser> a través de la inyección de dependencias.
        //Esta es una práctica común en ASP.NET Core para administrar las 
        //dependencias de los controladores.
        public AccountController(UserManager<AppUser> userManager,
            ITokenService tokenService, 
            SignInManager<AppUser> signInManager,
            IEmailService emailService
            )
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
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

                // Validación de espacios en blanco
                if (registerReqDto.UserName?.Contains(" ") == true)
                {
                    return BadRequest(new { error = "El nombre de usuario no puede contener espacios en blanco" });
                }

                // Trim de los campos
                registerReqDto.UserName = registerReqDto.UserName?.Trim();
                registerReqDto.Email = registerReqDto.Email?.Trim();
                registerReqDto.Password = registerReqDto.Password?.Trim();

                if (string.IsNullOrWhiteSpace(registerReqDto.UserName) ||
                    string.IsNullOrWhiteSpace(registerReqDto.Email) ||
                    string.IsNullOrWhiteSpace(registerReqDto.Password))
                {
                    return BadRequest(new { error = "Los campos no pueden estar vacíos" });
                }

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


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
                return Ok(); // Siempre devolver OK por seguridad

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Aquí está el cambio - usar la URL del frontend
            var resetLink = $"http://localhost:4200/user/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

            // Aquí deberías implementar el envío de email con el link de recuperación
            await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);

            return Ok(new { message = "Si el email existe, recibirás un link para restablecer tu contraseña" });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
                return BadRequest("Usuario no encontrado");

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors);
        }
    }
}