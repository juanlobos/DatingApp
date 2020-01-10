using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppApi.Data;
using DatingAppApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace DatingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly IDatingRepository _rep;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository rep, IMapper mapper)
        {
            _rep = rep;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users= await _rep.GetUsers();
            var usersReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            return Ok(usersReturn);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _rep.GetUser(id);
            var userReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto updateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userFromRepo = await _rep.GetUser(id);
            _mapper.Map(updateDto, userFromRepo);

            if (await _rep.SaveAll())
            {
                return NoContent();
            }
            throw new Exception($"Los datos del usuario {id} no se han guardado");
        }
    }
}