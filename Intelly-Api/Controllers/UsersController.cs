using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Implementations;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

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
        [Authorize]
        [Route("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            ApiResponse<List<UserEnt>> response = new ApiResponse<List<UserEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var users = await context.QueryAsync<UserEnt>("GetAllUsers", commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = users.ToList();
                    return Ok(response);
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetSpecificUser/{UserId}")]
        public async Task<IActionResult> GetSpecificUser(int UserId)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var user = await context.QueryFirstOrDefaultAsync<UserEnt>("GetSpecificUser",
                        new { UserId }, commandType: CommandType.StoredProcedure);

                    if (user != null)
                    {
                        response.Success = true;
                        response.Data = user;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "User not found";
                        response.Code = 404;
                        return NotFound(response);
                    }
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

        [HttpPut]
        [Authorize]
        [Route("EditSpecificUser")]
        public async Task<IActionResult> EditSpecificUser(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("EditSpecificUser",
                        new {
                            entity.User_Id,
                            entity.User_Company_Id,
                            entity.User_Name,
                            entity.User_LastName,
                            entity.User_Email,
                            entity.User_Type
                        },
                        commandType: CommandType.StoredProcedure);

                    if (data > 0)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "User not found";
                        response.Code = 404;
                        return NotFound(response);
                    }
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("GetAllUsersRoles")]
        public async Task<IActionResult> GetAllUsersRoles()
        {
            ApiResponse<List<UserRoleEnt>> response = new ApiResponse<List<UserRoleEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryAsync<UserRoleEnt>("GetAllUsersRoles", commandType: CommandType.StoredProcedure);
                    response.Success = true;
                    response.Data = data.ToList();
                    return Ok(response);
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                response.Code = 500;
                return BadRequest(response);
            }
        }

    }
}
