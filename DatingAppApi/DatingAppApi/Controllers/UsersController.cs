using System.Threading.Tasks;
using DatingAppApi.Data;
using Microsoft.AspNetCore.Mvc;

namespace DatingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IDatingRepository _rep;

        public UsersController(IDatingRepository rep)
        {
            _rep = rep;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users= await _rep.GetUsers();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _rep.GetUser(id);
            return Ok(user);
        }
    }
}