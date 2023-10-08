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
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var companiesData = await context.QueryAsync<CompanyEnt>("GetAllCompanies", commandType: CommandType.StoredProcedure);
                    return Ok(companiesData);
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("GetSpecificCompany/{CompanyId}")]
        public async Task<IActionResult> GetSpecificCompany(long CompanyId)
        {
            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var companyData = await context.QueryFirstOrDefaultAsync<CompanyEnt>("GetSpecificCompany",
                        new { CompanyId }, commandType: CommandType.StoredProcedure);

                    if (companyData != null)
                    {
                        return Ok(companyData);
                    }
                    else
                    {
                        return NotFound("Company not found");
                    }
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("CreateCompany")]
        public async Task<IActionResult> CreateCompany(CompanyEnt entity)
        {
            try
            {
                if (string.IsNullOrEmpty(entity.Company_Name) || string.IsNullOrEmpty(entity.Company_Email) || string.IsNullOrEmpty(entity.Company_Phone))
                {
                    return BadRequest("Name, email and phone are required.");
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    string connectionString = "";

                    if (string.IsNullOrEmpty(entity.Company_Connection_String))
                    {
                         connectionString = "waiting to be defined";
                    }
                    else
                    {
                        connectionString = entity.Company_Connection_String;
                    } 

                    var data = await context.ExecuteAsync("CreateCompany",
                        new { entity.Company_Name, entity.Company_Email, entity.Company_Phone, connectionString},
                        commandType: CommandType.StoredProcedure);

                        return Ok("Success");
                }
            }
            catch (SqlException ex)
            {
                return BadRequest("Unexpected Error: " + ex.Message);
            }
        }
    }
}
