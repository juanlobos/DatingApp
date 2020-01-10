using System;
using Microsoft.AspNetCore.Http;

namespace DatingAppApi.ViewModels
{
    public class FotosForCreationDto
    {
        public string Url  { get; set; }
        public IFormFile File { get; set; }
        public string Description { get; set; }
        public DateTime DateAdd { get; set; }
        public string PublicId { get; set; }

        public FotosForCreationDto()
        {
            DateAdd = DateTime.Now;
        }
    }
}