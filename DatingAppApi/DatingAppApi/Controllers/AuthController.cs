using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingAppApi.Data;
using DatingAppApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;

namespace DatingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _rep;
        private readonly IConfiguration _conf;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository rep, IConfiguration conf, IMapper mapper)
        {
            _mapper = mapper;
            _rep = rep;
            _conf = conf;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterViewModels register)
        {
            register.Username = register.Username.ToLower();
            if (await _rep.UserExists(register.Username))
            {
                return BadRequest("Usuario ya existente");
            }

            var userToCreate = new User { Username = register.Username };

            var createUser = await _rep.Register(userToCreate, register.Password);
            return StatusCode(201);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginViewModels login)
        {
            var userFromRepo = await _rep.Login(login.Username.ToLower(), login.Password);

            if (userFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name,userFromRepo.Username)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_conf.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new { token = tokenHandler.WriteToken(token),user });
        }



    }



}
