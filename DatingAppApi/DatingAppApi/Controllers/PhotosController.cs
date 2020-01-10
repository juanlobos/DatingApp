using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingAppApi.Data;
using DatingAppApi.Helpers;
using DatingAppApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingAppApi.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _rep;
        private readonly IMapper _map;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository rep, IMapper map, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _rep = rep;
            _map = map;
            _cloudinaryConfig = cloudinaryConfig;

            Account acc = new Account(_cloudinaryConfig.Value.CloudName, _cloudinaryConfig.Value.ApiKey, cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetFoto")]
        public async Task<IActionResult> GetFoto(int id)
        {
            var fotoFromRepo = await _rep.GetPhoto(id);
            var foto = _map.Map<FotoForReturnDto>(fotoFromRepo);
            return Ok(foto);
        }

        [HttpPost]
        public async Task<IActionResult> AddFotoUser(int userId, [FromForm]FotosForCreationDto fotosFor)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var userFromRepo = await _rep.GetUser(userId);
            var file = fotosFor.File;
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            fotosFor.Url = uploadResult.Uri.ToString();
            fotosFor.PublicId = uploadResult.PublicId;
            var foto = _map.Map<Photo>(fotosFor);
            userFromRepo.Photos.Add(foto);

            if (!userFromRepo.Photos.Any(u => u.IsMain))
            {
                foto.IsMain = true;
            }
            if (await _rep.SaveAll())
            {
                var fotoToReturn = _map.Map<FotoForReturnDto>(foto);
                return CreatedAtRoute("GetFoto", new { id = foto.Id }, fotoToReturn);
            }
            return BadRequest("No puede agregar una foto");
        }
        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user = await _rep.GetUser(userId);
            if (!user.Photos.Any(p=>p.Id==id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _rep.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("Esta es la foto ya principal");
            }
            var currentMainPhoto = await _rep.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;
            if (await _rep.SaveAll())
            {
                return NoContent();
            }
            return BadRequest("No se pudo establecer la foto a principal");
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var user = await _rep.GetUser(userId);
            if (!user.Photos.Any(p => p.Id == id))
            {
                return Unauthorized();
            }
            var photoFromRepo = await _rep.GetPhoto(id);
            if (photoFromRepo.IsMain)
            {
                return BadRequest("No puede borrar su foto principal");
            }
            if (photoFromRepo.PublicId != null)
            {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                if (result.Result == "ok")
                {
                    _rep.Delete(photoFromRepo);
                }
            }
            if (photoFromRepo.PublicId == null)
            {
                _rep.Delete(photoFromRepo);
            }


            
            if (await _rep.SaveAll())
            {
                return Ok();
            }
            return BadRequest("No puede borrar la foto");
        }
    }
}