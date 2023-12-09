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
    public class CustomerController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;

        public CustomerController(IDbConnectionProvider connectionProvider, ITools tools)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
        }

        [HttpGet]
        [Authorize]
        [Route("GetAllCustomers/{Customer_Company_Id}")] //Get all cutomers from x company, please provide company id
        public async Task<IActionResult> GetAllCustomers(string Customer_Company_Id)
        {
            ApiResponse<List<CustomerEnt>> response = new ApiResponse<List<CustomerEnt>>();

            try
            {
                string userToken = string.Empty;
                string userType = string.Empty;
                bool isAdmin = false;
                _tools.ObtainClaims(User.Claims, ref userToken, ref userType, ref isAdmin);

                if (!isAdmin)

                    return Unauthorized();
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryAsync<CustomerEnt>("GetAllCustomers", //Need SP
                        new { Customer_Company_Id }, commandType: CommandType.StoredProcedure);

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
        [Authorize]
        [Route("NewCustomer")]
        public async Task<IActionResult> NewCustomer(CustomerEnt entity) //Need SP
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                string userToken = string.Empty;
                string userType = string.Empty;
                bool isAdmin = false;
                _tools.ObtainClaims(User.Claims, ref userToken, ref userType, ref isAdmin);

                if (!isAdmin)

                    return Unauthorized();
                using (var context = _connectionProvider.GetConnection())
                {
                    var today = DateTime.Now;

                    var data = await context.ExecuteAsync("NewCustomer",
                        new {entity.Customer_Company_Id, entity.Customer_Name, entity.Customer_Email, 
                            Customer_Registration_Date = today},
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
        [Authorize]
        [Route("GetSpecificCustomer/{CustomerId}")]
        public async Task<IActionResult> GetSpecificCustomer(long CustomerId)
        {
            ApiResponse<CustomerEnt> response = new ApiResponse<CustomerEnt>();

            try
            {
                string userToken = string.Empty;
                string userType = string.Empty;
                bool isAdmin = false;
                _tools.ObtainClaims(User.Claims, ref userToken, ref userType, ref isAdmin);

                if (!isAdmin)

                    return Unauthorized();

                using (var context = _connectionProvider.GetConnection())
                {
                    var companyData = await context.QueryFirstOrDefaultAsync<CustomerEnt>("GetSpecificCompany",
                        new { CustomerId }, commandType: CommandType.StoredProcedure);

                    if (companyData != null)
                    {
                        response.Success = true;
                        response.Data = companyData;

                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Company not found";
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
        [Route("EditSpecificCustomer")]
        public async Task<IActionResult> EditSpecificCustomer(CustomerEnt entity)
        {
            ApiResponse<CustomerEnt> response = new ApiResponse<CustomerEnt>();

            try
            {
                string userToken = string.Empty;
                string userType = string.Empty;
                bool isAdmin = false;
                _tools.ObtainClaims(User.Claims, ref userToken, ref userType, ref isAdmin);

                if (!isAdmin)

                    return Unauthorized();

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("EditSpecificCompany",
                        new
                        {
                            entity.Customer_Company_Id,
                            entity.Customer_Name,
                            entity.Customer_Email,
                            entity.Customer_Membership_Level
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
        [HttpPut]
        [Authorize]
        [Route("UpdateCustomerState")]
        public async Task<IActionResult> UpdateCustomerState(UserEnt entity)
        {


            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("UpdateCustomerState",
                       new { entity.User_Id },
                       commandType: CommandType.StoredProcedure);

                    return Ok(data);

                }
            }
            catch (SqlException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
