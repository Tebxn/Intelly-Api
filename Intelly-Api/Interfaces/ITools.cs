using System.Data;

namespace Intelly_Api.Interfaces
{
    public interface ITools
    {
        String CreatePassword(int length);
        void SendEmail(string recipient, string subject, string body);
    }
}
