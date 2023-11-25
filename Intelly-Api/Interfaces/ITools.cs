using Intelly_Api.Entities;
using System.Data;
using System.Security.Claims;

namespace Intelly_Api.Interfaces
{
    public interface ITools
    {
        String CreatePassword(int length);
        bool SendEmail(string recipient, string subject, string body);
        public string GenerateToken(string userId);
        string MakeHtmlNewUser(UserEnt userData, string temporalPassword);
        string MakeHtmlEmailAdvertisement(string body, string imageUrl);
        string Encrypt(string texto);
        string Decrypt(string texto);
        public void ObtainClaims(IEnumerable<Claim> valores, ref string username, ref string userrol, ref bool isAdmin);
        public void ObtainClaimsID(IEnumerable<Claim> values, ref string userId);
    }
}
