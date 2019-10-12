using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingAppApi.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _db;

        public AuthRepository(DataContext db)
        {
            _db = db;
        }
        public async Task<User> Login(string username, string password)
        {
            var user=await _db.Users.FirstOrDefaultAsync(z=>z.Username==username);

            if(user == null)
            return null;

            if (!VerifyPasswordHash(password,user.PaswordHash,user.PaswordSalt))
            return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] paswordHash, byte[] paswordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512(paswordSalt))
            {
                var computeHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computeHash.Length; i++)
                {
                    if (computeHash[i] != paswordHash[i])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash,passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PaswordHash=passwordHash;
            user.PaswordSalt=passwordSalt;

            await _db.AddAsync(user);
            await _db.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac=new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _db.Users.AnyAsync(x=>x.Username==username))
            {
                return true;
            }
            return false;
        }
    }
}