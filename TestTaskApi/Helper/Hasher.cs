using Konscious.Security.Cryptography;
using System.Text;

namespace TestTaskApi.Helper
{
    public class Hasher
    {
        public string HashPassword(string password)
        {
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Iterations = 64;
                argon2.MemorySize = 4096;
                argon2.DegreeOfParallelism = 4;

                byte[] hashBytes = argon2.GetBytes(10);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
