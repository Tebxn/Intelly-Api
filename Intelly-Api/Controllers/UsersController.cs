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
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var users = await context.QueryAsync<UserEnt>("GetAllUsers", commandType: CommandType.StoredProcedure);
                    return Ok(users);
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSpecificUser/{UserId}")]
        public async Task<IActionResult> GetSpecificUser(int UserId)
        {
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var user = await context.QueryFirstOrDefaultAsync<UserEnt>("GetSpecificUser",
                        new { UserId }, commandType: CommandType.StoredProcedure);

                    if (user != null)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        return NotFound("User not found");
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }
        [HttpPut]
        [Route("EditSpecificUser")]
        public async Task<IActionResult> EditSpecificUser(UserEnt entity)
        {
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var user = await context.QueryFirstOrDefaultAsync<UserEnt>("EditSpecificUser",
                        new { entity.User_Id }, commandType: CommandType.StoredProcedure);

                    if (user != null)
                    {
                        return Ok(user);
                    }
                    else
                    {
                        return NotFound("User not found");
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }


    }
}
