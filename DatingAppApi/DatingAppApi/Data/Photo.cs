using System;
namespace DatingAppApi.Data
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Descripcion { get; set; }
        public DateTime DateAdd { get; set; }
        public bool IsMain { get; set; }
        public int UserId{ get; set; }
        public User User { get; set; }
    }
}