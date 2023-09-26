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
    public class AuthenticationController : ControllerBase
    {
        [HttpPost]
        [Route("SignIn")]
        public async Task<IActionResult> SignIn(UserEnt entity)
        {
            try
            {
                using (var context = new SqlConnection("Server=DESKTOP-0EDQFH8\\SQLEXPRESS;Database=Intelly_DB;Trusted_Connection=True MultipleActiveResultSets=true;"))
                {
                    var data = await context.QueryAsync<UserEnt>(
                        "SignIn",
                        new { entity.User_Email, entity.User_Password },
                        commandType: CommandType.StoredProcedure
                    );
                    return Ok(data.FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}

