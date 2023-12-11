using Dapper;
using Intelly_Api.Entities;
using Intelly_Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Intelly_Api.Implementations;
using Intelly_Web.Entities;

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

        [HttpPost]
        [Authorize]
        [Route("AddProduct")]
        public async Task<IActionResult> AddProduct(ProductEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("AddProduct",
                        new { entity.Product_Name, entity.Product_CompanyId, entity.Product_Internal_Code, entity.Product_Price,entity.Product_ImageUrl },
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
        [AllowAnonymous]
        [Route("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            ApiResponse<List<ProductEnt>> response = new ApiResponse<List<ProductEnt>>();

            try
            {
                
                using (var context = _connectionProvider.GetConnection())
                {
                    var productsData = await context.QueryAsync<ProductEnt>("GetAllProducts", commandType: CommandType.StoredProcedure);

                    response.Success = true;
                    response.Data = productsData.ToList();

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

        //[HttpGet]
        //[AllowAnonymous]
        //[Route("GetSpecificProduct/{ProductId}")]
        //public async Task<IActionResult> GetSpecificProduct(long ProductId)
        //{
        //    ApiResponse<ProductEnt> response = new ApiResponse<ProductEnt>();

        //    try
        //    {
        //        using (var context = _connectionProvider.GetConnection())
        //        {
        //            var companyData = await context.QueryFirstOrDefaultAsync<ProductEnt>("GetSpecificProduct",
        //                new { Product_Id = ProductId }, commandType: CommandType.StoredProcedure);

        //            if (companyData != null)
        //            {
        //                response.Success = true;
        //                response.Data = companyData;

        //                return Ok(response);
        //            }
        //            else
        //            {
        //                response.ErrorMessage = "Company not found";
        //                response.Code = 404;

        //                return NotFound(response);
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        response.ErrorMessage = "Unexpected Error: " + ex.Message;
        //        response.Code = 500;

        //        return BadRequest(response);
        //    }
        //}

        [HttpGet]
        [AllowAnonymous]
        [Route("GetSpecificProduct/{ProductId}")]
        public async Task<IActionResult> GetSpecificProduct(long ProductId)
        {
            ApiResponse<ProductEnt> response = new ApiResponse<ProductEnt>();

            try
            {
                using (var context = _connectionProvider.GetConnection())
                {
                    var localData = await context.QueryFirstOrDefaultAsync<ProductEnt>("GetSpecificProduct",
                        new { ProductId }, commandType: CommandType.StoredProcedure);

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

        [HttpPut]
        [Authorize]
        [Route("EditSpecificProduct")]
        public async Task<IActionResult> EditSpecificProduct(ProductEnt entity)
        {
            ApiResponse<ProductEnt> response = new ApiResponse<ProductEnt>();

            try
            {

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("EditSpecificProduct",
                        new
                        {
                            entity.Product_Id,
                            entity.Product_CompanyId,
                            entity.Product_Internal_Code,
                            entity.Product_Name,
                            entity.Product_Price  
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
    }
}
