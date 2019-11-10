using System;

namespace DatingAppApi.ViewModels
{
    public class PhotosForDetailsDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdd { get; set; }
        public bool IsMain { get; set; }
    }
}