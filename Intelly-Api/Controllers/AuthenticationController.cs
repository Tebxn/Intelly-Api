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
        [Route("IniciarSesion")]
        public IActionResult IniciarSesion(UserEnt entity)
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var data = context.Query<UserEnt>("IniciarSesion",
                        new { entity.User_Id, entity.User_Email, entity.User_Password },
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
        [Route("RegistrarCuenta")]
        public IActionResult RegistrarCuenta(UserEnt entity)
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var data = context.Execute("RegistrarCuenta",
                        new { entity.User_Id, entity.User_Name, entity.User_Email, entity.User_Password, entity.User_State },
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
        [Route("RecuperarCuenta")]
        public IActionResult RecuperarCuenta(UserEnt entity)
        {
            try
            {
                using (var context = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
                {
                    var data = context.Query<UserEnt>("RecuperarCuenta",
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

