using HomeBanking.Models;
using HomeBanking.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;

namespace HomeBanking.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IClientRepository _clientRepository;

        public AuthController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // Maneja la acción de inicio de sesión
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Client client)
        {
            try
            {
                // Busca al usuario en la base de datos por su correo electrónico
                Client user = _clientRepository.FindByEmail(client.Email);
                if (user == null || !String.Equals(user.Password, client.Password))
                    return Unauthorized(); // Retorna una respuesta de autenticación no autorizada (401)

                // Crea una lista de reclamos (claims) para el usuario
                var claims = new List<Claim>
                {
                    new Claim("Client", user.Email),
                };

                // Crea una identidad de reclamos (claims) para el usuario
                var claimsIdentity = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                // Realiza el inicio de sesión del usuario. Le envia  todos los datos para que cree la cookie y la mande al navegador para que la persona se loguee
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return Ok(); // Retorna una respuesta exitosa (200 OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Retorna un error interno del servidor (500)
            }
        }

        // Maneja la acción de cierre de sesión
        [HttpPost("logout")] /*Endpoint de tipo post que se llama logout*/
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Realiza el cierre de sesión del usuario
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                return Ok(); // Retorna una respuesta exitosa (200 OK)
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); // Retorna un error interno del servidor (500)
            }
        }
    }
}
