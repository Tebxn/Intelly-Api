using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Intelly_Api.Controllers
{
    public class BuyController : Controller
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public BuyController(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        [HttpPost]
        [Authorize]
        [Route("NewBuy")]
        public async Task<IActionResult> NewBuy(CustomerEnt entity) //Need SP
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("NewBuy",
                        new { entity.Customer_Company_Id, entity.Customer_Name, entity.Customer_Email, entity.Customer_Membership_Level },
                        commandType: CommandType.StoredProcedure);

                    if (data > 0)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error with the input customer data";
                        response.Code = 500;
                        return BadRequest(response);
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
    }
}
