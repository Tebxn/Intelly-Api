using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDbConnectionProvider _connectionProvider;

        public ProductController(IDbConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetAllProducts/{Product_CompanyId}")]
        public async Task<IActionResult> GetAllProducts(long product_CompanyId)
        {
            ApiResponse<List<ProductEnt>> response = new ApiResponse<List<ProductEnt>>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var users = await context.QueryAsync<ProductEnt>("GetAllProducts",
                        new { product_CompanyId },commandType: CommandType.StoredProcedure);

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

        [HttpGet]
        [AllowAnonymous]
        [Route("GetSpecificProduct/{ProductId}")]
        public async Task<IActionResult> GetSpecificProduct(int UserId)
        {
            ApiResponse<ProductEnt> response = new ApiResponse<ProductEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var user = await context.QueryFirstOrDefaultAsync<ProductEnt>("GetSpecificProduct",
                        new { UserId }, commandType: CommandType.StoredProcedure);

                    if (user != null)
                    {
                        response.Success = true;
                        response.Data = user;
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
    }
}
