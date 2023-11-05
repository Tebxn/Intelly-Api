using Intelly_Api.Entities;
using Intelly_Api.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Intelly_Api.Interfaces;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailMarketingController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;
        private readonly IBCryptHelper _bCryptHelper;

        public EmailMarketingController(IDbConnectionProvider connectionProvider, ITools tools)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetAllMarketingCampaigns/{MarketingCampaign_CompanyId}")]
        public async Task<IActionResult> GetAllUsers(string MarketingCampaign_CompanyId)
        {
            ApiResponse<List<MarketingCampaignEnt>> response = new ApiResponse<List<MarketingCampaignEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {

                    var data = await context.QueryAsync<MarketingCampaignEnt>("GetAllMarketingCampaigns",new { MarketingCampaign_CompanyId },
                        commandType: CommandType.StoredProcedure);

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

        [HttpPost]
        [Authorize]
        [Route("EmailMarketingManual")]
        public async Task<IActionResult> EmailMarketingManual(EmailEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {

                    var data = await context.QueryAsync<CustomerEnt>("GetAllCustomers",
                       new { entity.CompanySenderId },
                       commandType: CommandType.StoredProcedure);

                    string companyName = entity.CompanyName;
                    string body = entity.Body;
                    

                    foreach (var item in data) 
                    {
                        string recipient = item.Customer_Email;
                        _tools.SendEmail(recipient, companyName, body);
   
                    }
                    response.Success = true;
                    response.Code = 200;
                    return Ok(response);
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Error sending email marketing";
                response.Code = 500;
                return BadRequest(response);
            }
        }
    }
}
