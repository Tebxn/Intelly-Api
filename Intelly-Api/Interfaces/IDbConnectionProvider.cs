using System.Data;

namespace Intelly_Api.Interfaces
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetConnection();
    }
}
