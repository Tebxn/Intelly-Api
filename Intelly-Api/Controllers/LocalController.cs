using Intelly_Api.Entities;
using Intelly_Api.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Intelly_Web.Entities;
using Intelly_Api.Interfaces;
using Dapper;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalController : ControllerBase
    {

        private readonly IDbConnectionProvider _connectionProvider;

        public LocalController(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        [HttpGet]
        [Authorize]
        [Route("GetAllLocals")]
        public async Task<IActionResult> GetAllLocals()
        {
            ApiResponse<List<LocalEnt>> response = new ApiResponse<List<LocalEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var users = await context.QueryAsync<LocalEnt>("GetAllLocals", commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = users.ToList();
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
        [Route("CreateLocal")]
        public async Task<IActionResult> CreateLocal(LocalEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("CreateLocal",
                        new { entity.Intern_Id, entity.Local_Name, entity.Location },
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

        [HttpPut]
        [Authorize]
        [Route("EditSpecificLocal")]
        public async Task<IActionResult> EditSpecificLocal(LocalEnt entity)
        {
            ApiResponse<LocalEnt> response = new ApiResponse<LocalEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("EditSpecificLocal",
                        new
                        {
                            entity.Local_Id,
                            entity.Intern_Id,
                            entity.Local_Name,
                            entity.Location
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
        [Route("DisableLocal")] //Need SP
        public async Task<IActionResult> DisableLocal(LocalEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (entity.Local_Id == 0)
                {
                    response.ErrorMessage = "Local Id can't be empty.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("DisableLocal",
                        new { entity.Local_Id },
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


        [HttpGet]
        [Authorize]
        [Route("GetSpecificLocal/{LocalId}")]
        public async Task<IActionResult> GetSpecificLocal(long LocalId)
        {
            ApiResponse<LocalEnt> response = new ApiResponse<LocalEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var localData = await context.QueryFirstOrDefaultAsync<LocalEnt>("GetSpecificLocal",
                        new { LocalId }, commandType: CommandType.StoredProcedure);

                    if (localData != null)
                    {
                        response.Success = true;
                        response.Data = localData;

                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Local not found";
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
