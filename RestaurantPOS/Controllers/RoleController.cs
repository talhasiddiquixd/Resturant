using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public RoleController(IConfiguration config, IRoleRepository roleRepository, IErrorLogRepository errorLogRepository)
        {
            _roleRepository = roleRepository;
            _errorLogRepository = errorLogRepository;
            _config = config;
        }



        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all User Role", OperationId = "Get all")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                var responseData = await _roleRepository.Get();
                if (responseData != null && responseData.Count > 0)
                {
                    response.StatusCode = 200;
                    response.Data = responseData;
                    response.TotalRecords = responseData.Count;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RoleController", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        [HttpGet("GetAllPremissionOfSpecificRole/{Id}")]
        [SwaggerOperation(Summary = "Get all User Role", OperationId = "Get premission of specific role")]
        public async Task<IActionResult> GetAllPremissionOfSpecificRole(int Id)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                var responseData = await _roleRepository.GetPremisionOfRole(Id);
                if (responseData != null && responseData.Count > 0)
                {
                    response.StatusCode = 200;
                    response.Data = responseData;
                    response.TotalRecords = responseData.Count;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RoleController", "GetAllPremissionOfSpecificRole");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        [HttpGet("GetById/{RoleId}")]
        [SwaggerOperation(Summary = "Get specific Role", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int RoleId)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _roleRepository.Get(RoleId);
                if (responseData != null)
                {
                    response.StatusCode = 200;
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RoleController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save new Role or update existing Role", OperationId = "Save/Update Role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Post([FromBody] RoleRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.Name))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideRoleName;
                    }
                    if (response.IsSuccess)
                    {
                        var saveFoodCategory = await _roleRepository.Save(model, Convert.ToInt32(loginUserId));
                        var ListData = await _roleRepository.Get();
                        response.StatusCode = 200;
                        response.Data = model;
                        response.TotalRecords = ListData.Count();
                        response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RoleController", "Save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        [HttpPost("AssignRoleToUser")]
        [SwaggerOperation(Summary = "Asssign new Role or update existing Role of user", OperationId = "Assign/Update user role")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AssignRoleToUser([FromBody] UserRoleRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                if (model != null)
                {
                    if (model.RoleId <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideRoleName;
                    }
                    else if (model.UserId <= 0)
                    { 
                    }
                    if (response.IsSuccess)
                    {
                        var saveFoodCategory = await _roleRepository.AssignRoleToUser(model, Convert.ToInt32(loginUserId));
                        var ListData = await _roleRepository.Get();
                        response.StatusCode = 200;
                        response.Data = model;
                        response.TotalRecords = ListData.Count();
                        response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RoleController", "AssignRoleToUser");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        [HttpGet("GetAllUserRole")]
        [SwaggerOperation(Summary = "Get all User Role", OperationId = "List of user role")]
        public async Task<IActionResult> GetAllUserRole()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                var responseData = await _roleRepository.GetAllUserRole();
                if (responseData != null && responseData.Count > 0)
                {
                    response.StatusCode = 200;
                    response.Data = responseData;
                    response.TotalRecords = responseData.Count;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RoleController", "GetAllUserRole");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }




        // Delete...
        [HttpDelete("DeleteRole/{id}")]
        [SwaggerOperation(Summary = "Delete role by Id", OperationId = "Delete role")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _roleRepository.Delete(id, Convert.ToInt32(loginUserId));
                if (responseData > 0)
                {
                    var ListData = await _roleRepository.Get();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    var ListData = await _roleRepository.Get();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RoleController", "DeleteRole");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpDelete("DeleteRoleAssign/{id}")]
        [SwaggerOperation(Summary = "Delete roleAssign by Id", OperationId = "Delete roleAssign")]
        public async Task<IActionResult> DeleteRoleAssign(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _roleRepository.DeleteRoleAssign(id, Convert.ToInt32(loginUserId));
                if (responseData > 0)
                {
                    var ListData = await _roleRepository.Get();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    var ListData = await _roleRepository.Get();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "Delete");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


    }
}
