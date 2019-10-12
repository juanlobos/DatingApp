using System.Threading.Tasks;
using DatingAppApi.Data;
using DatingAppApi.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _rep;

        public AuthController(IAuthRepository rep)
        {
            _rep = rep;
        }
        public async Task<IActionResult> Register(RegisterViewModels register)
        {
            register.Username = register.Username.ToLower();
            if (await _rep.UserExists(register.Username))
            {
                return BadRequest("Usuario ya existente");
            }

            var userToCreate=new User{Username=register.Username};

            var createUser=await _rep.Register(userToCreate,register.Password);
            return StatusCode(201);
        }

    }
}