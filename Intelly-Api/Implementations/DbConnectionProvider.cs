using Intelly_Api.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Intelly_Api.Implementations
{
    public class DbConnectionProvider : IDbConnectionProvider
    {
        private readonly IConfiguration _configuration;
        public DbConnectionProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection GetConnection()
        {
            return new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
