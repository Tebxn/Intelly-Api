using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Intelly_Api.Entities;
using Microsoft.AspNetCore.Hosting.Server;
using Org.BouncyCastle.Cms;
using Intelly_Api.Interfaces;
using Intelly_Api.Implementations;
using System.Data.Common;
using BCrypt.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;

namespace Intelly_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly IDbConnectionProvider _connectionProvider;
        private readonly ITools _tools;
        private readonly IBCryptHelper _bCryptHelper;

        public AuthenticationController(IDbConnectionProvider connectionProvider, ITools tools, IBCryptHelper bCryptHelper)
        {
            _connectionProvider = connectionProvider;
            _tools = tools;
            _bCryptHelper = bCryptHelper;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login(UserEnt entity)
        {
            ApiResponse<UserEnt> response = new ApiResponse<UserEnt>();

            try
            {
                if (string.IsNullOrEmpty(entity.User_Email) || string.IsNullOrEmpty(entity.User_Password))
                {
                    response.ErrorMessage = "Email and password are required";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var connection = _connectionProvider.GetConnection())
                {
                    var data = await connection.QueryFirstOrDefaultAsync<UserEnt>("Login",
                        new { entity.User_Email},
                        commandType: CommandType.StoredProcedure);

                    //Check password
                    bool validPassword = _bCryptHelper.CheckPassword(entity.User_Password, data.User_Password);

                    if (data == null || !validPassword)
                    {
                        response.ErrorMessage = "Incorrect email or password";
                        response.Code = 404;
                        return NotFound(response);
                    }

                    response.Success = true;
                    response.Code = 200;
                    response.Data = data;
                    response.Data.UserToken = _tools.GenerateToken(data.User_Id.ToString());
                    return Ok(response);
                }
            }
            catch (SqlException ex)
            {
                response.ErrorMessage = "Unexpected Error: " + ex.Message;
                return BadRequest(response);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterAccount")]
        public async Task<IActionResult> RegisterAccount(UserEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (string.IsNullOrEmpty(entity.User_Name) || string.IsNullOrEmpty(entity.User_LastName) || string.IsNullOrEmpty(entity.User_Email))
                {
                    response.ErrorMessage = "Name, lastname, and email are required.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                //HashRandomPassword
                var randomPassword = _tools.CreatePassword(8);
                var hashedPassword = _bCryptHelper.HashPassword(randomPassword);
                entity.User_Password = hashedPassword;

                using (var context = _connectionProvider.GetConnection())
                {
                    string body = _tools.MakeHtmlNewUser(entity, randomPassword);
                    string recipient = entity.User_Email;

                    bool emailIsSend = _tools.SendEmail(recipient, "Intelly New Account", body);
                    if (emailIsSend)
                    {
                        var data = await context.ExecuteAsync("RegisterAccount",
                        new { entity.User_Name, entity.User_LastName, entity.User_Email, entity.User_Password, entity.User_Type, entity.User_State, entity.User_Company_Id },
                        commandType: CommandType.StoredProcedure);

                        if (data != 0)
                        {
                            response.Success = true;
                            response.Code = 200;
                            return Ok(response);
                        }
                        else
                        {
                            response.ErrorMessage = "Error saving new user";
                            response.Code = 500;
                            return BadRequest(response);
                        }
                    }
                    else 
                    {
                        response.ErrorMessage = "Error Sending email";
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

        [HttpPost]
        [AllowAnonymous]
        [Route("RecoverAccount")]
        public async Task<IActionResult> RecoverAccount(UserEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (string.IsNullOrEmpty(entity.User_Email))
                {
                    response.ErrorMessage = "Email is required.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                string temporalPassword = _tools.CreatePassword(8);

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.QueryFirstOrDefaultAsync<UserEnt>("RecoverAccount",
                        new { entity.User_Email, TemporalPassword = temporalPassword },
                        commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        string body = "Your new password to access Intelly CRM is: " + temporalPassword +
                            "\nPlease log in with your new password and change it.";
                        string recipient = entity.User_Email;
                        _tools.SendEmail(recipient, "Intelly Recover Account", body);

                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.ErrorMessage = "Error Sending email";
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

        //[HttpPut]
        //[Authorize]
        //[Route("DisableAccount")]
        //public async Task<IActionResult> DisableAccount(UserEnt entity)
        //{
        //    ApiResponse<string> response = new ApiResponse<string>();

        //    try
        //    {
        //        if (entity.User_Id == 0)
        //        {
        //            response.ErrorMessage = "User_Id can't be empty.";
        //            response.Code = 400;
        //            return BadRequest(response);
        //        }

        //        using (var context = _connectionProvider.GetConnection())
        //        {
        //            var data = await context.ExecuteAsync("DisableAccount",
        //                new { entity.User_Id },
        //                commandType: CommandType.StoredProcedure);

        //            if (data > 0) 
        //            {
        //                response.Success = true;
        //                response.Code = 200;
        //                return Ok(response);
        //            }
        //            else
        //            {
        //                response.Code = 500;
        //                response.ErrorMessage = "Error disabling account";
        //                return BadRequest(response);
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

        [HttpPut]
        [Authorize]
        [Route("ActivateAccount")]
        public async Task<IActionResult> ActivateAccount(int User_Id)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (User_Id == 0)
                {
                    response.ErrorMessage = "User_Id can't be empty.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var data = await context.ExecuteAsync("ActivateAccount",
                        new { User_Id = User_Id },
                        commandType: CommandType.StoredProcedure);

                    if (data > 0)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.Code = 500;
                        response.ErrorMessage = "Error activating account";
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
        [Route("UpdateUserPassword")]
        public async Task<IActionResult> UpdateUserPassword(UserEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (entity.User_Password == null)
                {
                    response.ErrorMessage = "Email can't be empty.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var hashedPassword = _bCryptHelper.HashPassword(entity.User_Password);

                    var data = await context.QueryFirstOrDefaultAsync<UserEnt>("UpdateUserPassword",
                        new { entity.User_Id, hashedPassword},
                        commandType: CommandType.StoredProcedure);

                    if (data != null)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.Code = 500;
                        response.ErrorMessage = "Error updating password";
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
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserEnt entity)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            try
            {
                if (entity.User_Id == 0 || string.IsNullOrEmpty(entity.User_Password))
                {
                    response.ErrorMessage = "User_Id and new password can't be empty.";
                    response.Code = 400;
                    return BadRequest(response);
                }

                using (var context = _connectionProvider.GetConnection())
                {
                    var newPassword = _bCryptHelper.HashPassword(entity.User_Password);

                    var data = await context.ExecuteAsync("ChangePassword", //me devuelve -1
                        new { entity.User_Id, newPassword },
                        commandType: CommandType.StoredProcedure);

                    if (data != 0)
                    {
                        response.Success = true;
                        response.Code = 200;
                        return Ok(response);
                    }
                    else
                    {
                        response.Code = 500;
                        response.ErrorMessage = "Error changing password";
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

