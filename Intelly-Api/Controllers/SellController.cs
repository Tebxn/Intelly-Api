using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Intelly_Api.Controllers
{
    public class SellController : Controller
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public SellController(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("NewSell")]
        public async Task<IActionResult> NewSell(SellEnt entity) //Need SP
        {
            ApiResponse<string> response = new ApiResponse<string>();
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    DateTime today = DateTime.Now;

                    var data = await context.ExecuteAsync("NewSell",
                        new
                        {
                            NumFactura = entity.NumFactura,
                            CustomerId = entity.CustomerId,
                            CompanyId = entity.CompanyId,
                            LocalId = entity.LocalId,
                            SellDate = today,
                            MarketingCampaignId = entity.MarketingCampaignId,
                            SellTotal = entity.Total
                        }, commandType: CommandType.StoredProcedure);

                    if (data > 0)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error with the input buy data";
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
