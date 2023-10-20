using System.Data;

namespace Intelly_Api.Interfaces
{
    public interface ITools
    {
        String CreatePassword(int length);
        bool SendEmail(string recipient, string subject, string body);
    }
}
