using System;
using System.Collections.Generic;

namespace DatingAppApi.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PaswordHash{get;set;}
        public byte[] PaswordSalt{get;set;}
        public string Gender { get; set; }
        public DateTime DateOfBith { get; set; }
        public string KnowAS { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photo { get; set; }
    }
}