namespace DatingAppApi.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PaswordHash{get;set;}
        public byte[] PaswordSalt{get;set;}
    }
}