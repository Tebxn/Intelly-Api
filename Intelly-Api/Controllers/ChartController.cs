using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Implementations;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;

        public ChartController(IDbConnectionProvider connectionProvider, ITools tools)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
        }

        [HttpGet]
        [Authorize]
        [Route("ChartNewCustomersMonth{companyId}")]
        public async Task<IActionResult> ChartNewCustomersMonth(long companyId)
        {
            ApiResponse<long> response = new ApiResponse<long>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<long>("ChartNewCustomersMonth",
                        new { Customer_Company_Id = companyId }, commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Data = data;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error consulting chart";
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
        [AllowAnonymous]
        [Route("ChartActivesMarketingCampaigns{companyId}")]
        public async Task<IActionResult> ChartActivesMarketingCampaigns(long companyId)
        {
            ApiResponse<long> response = new ApiResponse<long>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<long>("ChartActivesMarketingCampaigns",
                        new { Company_Id = companyId }, commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Data = data;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error consulting chart";
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
        [AllowAnonymous]
        [Route("ChartEmailsSendedMonth{companyId}")]
        public async Task<IActionResult> ChartEmailsSendedMonth(long companyId)
        {
            ApiResponse<long> response = new ApiResponse<long>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<long>("ChartEmailsSendedMonth",
                        new { CompanyId = companyId }, commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Data = data;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error consulting chart";
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
        [AllowAnonymous]
        [Route("ChartSellsWithCampaignMonth{companyId}")]
        public async Task<IActionResult> ChartSellsWithCampaignMonth(long companyId)
        {
            ApiResponse<long> response = new ApiResponse<long>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<long>("ChartSellsWithCampaignMonth",
                        new { CompanyId = companyId }, commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Data = data;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error consulting chart";
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
        [AllowAnonymous]
        [Route("ChartSumTotalByMonthActualYear{companyId}")]
        public async Task<IActionResult> ChartSumTotalByMonthActualYear(long companyId)
        {
            ApiResponse<List<ChartEnt>> response = new ApiResponse<List<ChartEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryAsync<ChartEnt>("ChartSumTotalByMonthActualYear",
                        new { CompanyId = companyId }, commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Data = data.ToList();
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error consulting chart";
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
    }
}
