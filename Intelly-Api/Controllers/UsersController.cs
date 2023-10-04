using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Implementations;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public UsersController(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;  
        }

        [HttpGet]
        [Route("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var users = context.Query<UserEnt>("GetAllUsers", commandType: CommandType.StoredProcedure);
                    return Ok(users);
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

    }
}
