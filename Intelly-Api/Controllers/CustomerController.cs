using Intelly_Api.Entities;
using Intelly_Api.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Intelly_Api.Interfaces;
using Dapper;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public CustomerController(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        [HttpGet]
        [Route("GetAllCustomers/{CompanyId}")] //Get all cutomers from x company, please provide company id
        public async Task<IActionResult> GetAllCustomers(long companyId)
        {
            ApiResponse<List<CustomerEnt>> response = new ApiResponse<List<CustomerEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryAsync<CustomerEnt>("GetAllCustomers", //Need SP
                        new {companyId}, commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Data = data.ToList();
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error consulting customers";
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

        [HttpPost]
        [Route("NewCustomer")]
        public async Task<IActionResult> NewCustomer(CustomerEnt entity) //Need SP
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("NewCustomer",
                        new {entity.Customer_Company_Id, entity.Customer_Name, entity.Customer_FirstLastname, 
                            entity.Customer_SecondLastname, entity.Customer_Email, entity.Customer_Membership_Level},
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
        [HttpGet]
        [Route("GetSpecificCustomer/{CompanyId}/{UserId}")]
        public async Task<IActionResult> GetSpecificCustumer(long companyId, long userId)
        {
            ApiResponse<CustomerEnt> response = new ApiResponse<CustomerEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<CustomerEnt>("GetSpecificCustumer", //Need SP
                        new { companyId, userId }, commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Data = data;

                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Customer not found";
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
