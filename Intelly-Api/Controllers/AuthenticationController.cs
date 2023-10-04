using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Intelly_Api.Entities;
using Microsoft.AspNetCore.Hosting.Server;
using Org.BouncyCastle.Cms;
using Intelly_Api.Interfaces;
using Intelly_Api.Implementations;
using System.Data.Common;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;

        public AuthenticationController(IDbConnectionProvider connectionProvider, ITools tools)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(UserEnt entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.User_Email) || string.IsNullOrEmpty(entity.User_Password))
                {
                    return BadRequest("Email and password are required");
                }

                using (var connection = _connectionProvider.GetConnection())
                {
                    var data = await connection.QueryFirstOrDefaultAsync<UserEnt>("Login",
                        new { entity.User_Email, entity.User_Password },
                        commandType: CommandType.StoredProcedure);

                    if (data == null)
                    {
                        return NotFound("Email or password incorrect");
                    }

                    return Ok(data);
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unespected Error: " + ex.Message);
            }
        }


        [HttpPost]
        [Route("RegisterAccount")]
        public async Task<IActionResult> RegisterAccount(UserEnt entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.User_Name) || string.IsNullOrEmpty(entity.User_LastName) || string.IsNullOrEmpty(entity.User_Email))
                {
                    return BadRequest("Name, lastname, email are required.");
                }

                entity.User_Password = _tools.CreatePassword(8);

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("RegisterAccount",
                        new { entity.User_Name, entity.User_LastName, entity.User_Email, entity.User_Password, entity.User_Type, entity.User_State, entity.User_Company_Id },
                        commandType: CommandType.StoredProcedure);

                    return Ok("Success");
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("RecoverAccount")]
        public async Task<IActionResult> RecoverAccount(UserEnt entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.User_Email))
                {
                    return BadRequest("Email is required.");
                }

                string temporalPassword = _tools.CreatePassword(8);

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<UserEnt>("RecoverAccount",
                        new { entity.User_Email, TemporalPassword = temporalPassword }, // Cambiado para incluir la temporalPassword
                        commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        string body = "Your new password to access Intelly CRM is: " + temporalPassword +
                            "\nPlease log in with your new password and change it.";
                        string recipient = entity.User_Email;
                        _tools.SendEmail(recipient, "Intelly Recover Account", body);
                    }

                    return Ok("Success");
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("DisableAccount")]
        public async Task<IActionResult> DisableAccount(UserEnt entity)
        {
            try
            {
                if (entity.User_Id == 0)
                {
                    return BadRequest("User_Id can't be empty.");
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<UserEnt>("DisableAccount",
                        new { entity.User_Id },
                        commandType: CommandType.StoredProcedure);

                    return Ok("Success");
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("ActivateAccount")]
        public async Task<IActionResult> ActivateAccount(UserEnt entity)
        {
            try
            {
                if (entity.User_Id == 0)
                {
                    return BadRequest("User_Id can't be empty.");
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<UserEnt>("ActivateAccount",
                        new { entity.User_Id },
                        commandType: CommandType.StoredProcedure);

                    return Ok("Success");
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

    }
}

