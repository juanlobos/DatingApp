using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppApi.Data;
using DatingAppApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;
using DatingAppApi.Helpers;

namespace DatingAppApi.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
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
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _rep.GetUser(currentUserId);
            userParams.UserId = currentUserId;
            if (String.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender=userFromRepo.Gender=="male" ? "female":"male";
            }
            var users= await _rep.GetUsers(userParams);
            var usersReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersReturn);
        }

        [HttpGet("{id}",Name="GetUser")]
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

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var like= await _rep.GetLike(id,recipientId);
            if (like != null)
            {
                return BadRequest("Ya le ha dado like a ese usuario");
            }
            if (await _rep.GetUser(id)==null)
            {
                return NotFound();
            }

            like=new Like{
                LikerId=id,
                LikeeId=recipientId
            };
            _rep.Add(like);
            if (await _rep.SaveAll())
            {
                return Ok();
            }
            return BadRequest("Failed to like User");
        }

    }
}