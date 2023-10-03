using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Intelly_Api.Entities;
using Microsoft.AspNetCore.Hosting.Server;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(UserEnt entity)
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var data = context.Query<UserEnt>("Login",
                        new { entity.User_Email, entity.User_Password },
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("RegisterAccount")]
        public IActionResult RegisterAccount(UserEnt entity)
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var data = context.Execute("RegisterAccount",
                        new { entity.User_Name, entity.User_LastName, entity.User_Email, entity.User_Password, entity.User_Type, entity.User_State, entity.User_Company_Id },
                        commandType: CommandType.StoredProcedure);

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("RecoverAccount")]
        public IActionResult RecoverAccount(UserEnt entity)
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var data = context.Query<UserEnt>("RecoverAccount",
                        new { entity.User_Email },
                        commandType: CommandType.StoredProcedure).FirstOrDefault();

                    return Ok(data);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

