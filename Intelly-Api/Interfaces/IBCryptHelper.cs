namespace Intelly_Api.Interfaces
{
    public interface IBCryptHelper
    {
        string HashPassword(string password);
        bool CheckPassword(string password, string hashedPassword);
    }
}
