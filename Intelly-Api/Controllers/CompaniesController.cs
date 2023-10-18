using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.ComponentModel.Design;
using Intelly_Api.Implementations;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public CompaniesController(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        [HttpGet]
        [Route("GetAllCompanies")]
        public async Task<IActionResult> GetAllCompanies()
        {
            ApiResponse<List<CompanyEnt>> response = new ApiResponse<List<CompanyEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var companiesData = await context.QueryAsync<CompanyEnt>("GetAllCompanies", commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = companiesData.ToList();

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

        [HttpGet]
        [Route("GetSpecificCompany/{CompanyId}")]
        public async Task<IActionResult> GetSpecificCompany(long CompanyId)
        {
            ApiResponse<CompanyEnt> response = new ApiResponse<CompanyEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var companyData = await context.QueryFirstOrDefaultAsync<CompanyEnt>("GetSpecificCompany",
                        new { CompanyId }, commandType: CommandType.StoredProcedure);

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

        [HttpPost]
        [Route("CreateCompany")]
        public async Task<IActionResult> CreateCompany(CompanyEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (string.IsNullOrEmpty(entity.Company_Name) || string.IsNullOrEmpty(entity.Company_Email) || string.IsNullOrEmpty(entity.Company_Phone))
                {
                    response.ErrorMessage = "Name, email and phone are required.";
                    response.Code = 400;

                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    string connectionString = string.IsNullOrEmpty(entity.Company_Connection_String) ? "waiting to be defined" : entity.Company_Connection_String;

                    var data = await context.ExecuteAsync("CreateCompany",
                        new { entity.Company_Name, entity.Company_Email, entity.Company_Phone, connectionString },
                        commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = "Success";

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

        [HttpPut]
        [Route("DisableCompany")] //Need SP
        public async Task<IActionResult> DisableCompany(CompanyEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (entity.Company_Id == 0)
                {
                    response.ErrorMessage = "Company Id can't be empty.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("DisableCompany",
                        new { entity.Company_Id},
                        commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Code = 200;
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

        [HttpPut]
        [Route("ActivateCompany")] //Need SP
        public async Task<IActionResult> ActivateCompany(CompanyEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (entity.Company_Id == 0)
                {
                    response.ErrorMessage = "Company Id can't be empty.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("ActivateCompany",
                        new { entity.Company_Id },
                        commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Code = 200;
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

    }
}
