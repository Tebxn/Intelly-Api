using Intelly_Api.Interfaces;

namespace Intelly_Api.Implementations
{
    public class BCryptHelper : IBCryptHelper
    {
        public string HashPassword(string password)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password); 
            return hashedPassword;
        }

        public bool CheckPassword(string password,string hashedPassword)
        { 
            bool validPassword = BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            return validPassword;
        }
    }
}
