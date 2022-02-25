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
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PremissionAssignController : ControllerBase
    {
        private readonly IPermissionAssignRepository _premissionAssignRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public PremissionAssignController(IPermissionAssignRepository premissionRepository, IConfiguration config, IErrorLogRepository errorLogRepository)//, ILogger<PermissionController> logger)
        {
            _config = config;
            _premissionAssignRepository = premissionRepository;
            _errorLogRepository = errorLogRepository;
        }


        [HttpPost("AssignPremissions")]
        [SwaggerOperation(Summary = "Assign Premissions to roles or update Premission", OperationId = "Assign/update Premission")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AssignPremissions([FromBody] AssignPermissionRequestModel model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                if (model.RoleId <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideRoleId;
                }
                if (model != null && response.IsSuccess)
                {
                    SaveResult = await _premissionAssignRepository.AssignPrimission(model.PermissionId, model.RoleId, Convert.ToInt32(loginUserId));
                }
                if (SaveResult >= 0)
                {
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionAssignController", "AssignPermissions");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpGet("GetAllRolePremission")]
        [SwaggerOperation(Summary = "List of GetAllRolePremission", OperationId = "GetAllRolePremission")]
        public async Task<IActionResult> GetAllRolePremission()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var RolePremissin = await _premissionAssignRepository.GetAllRolePremission();
                var RolePremissinCount = RolePremissin?.Count() ?? 0;
                if (RolePremissin != null)
                {
                    response.Data = RolePremissin;
                    response.TotalRecords = RolePremissinCount;
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
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionAssignControler", "GetAllRolePermission");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        //[HttpGet("GetRolePremissionbyid/{Id}")]
        //[SwaggerOperation(Summary = "Get Role Premissionby id", OperationId = "Get Role Premission by id")]
        //public async Task<IActionResult> GetRolePremission(int Id)
        //{
        //    ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
        //    string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
        //    try
        //    {
        //        var RolePremissin = await _premissionAssignRepository.GetRolePremission(Id);
        //        if (RolePremissin != null)
        //        {
        //            response.Data = RolePremissin;
        //            response.TotalRecords = 1;
        //            response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
        //        }
        //        else
        //        {
        //            response.StatusCode = 404;
        //            response.Message = ResponseMessageHelper.NoRecordFound;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = 404;
        //        response.IsSuccess = false;
        //        response.ExceptionMessage = "exception" + ex;
        //        new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserControler", "GetAll");
        //    }
        //    return ResponseHelper<object>.GenerateResponse(response);
        //}



        [HttpGet("GetAllPremissionOfAllRoles")]
        [SwaggerOperation(Summary = "List of All Premission Of All Roles", OperationId = "List of All Premission Of All Roles")]
        public async Task<IActionResult> GetAllPremissionOfAllRoles()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var RolePremissin = await _premissionAssignRepository.GetAllPremissionOfAllRoles();
                var RolePremissinCount = RolePremissin?.Count() ?? 0;
                if (RolePremissin != null)
                {
                    response.Data = RolePremissin;
                    response.TotalRecords = RolePremissinCount;
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
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionAssignControler", "GetAllPremissionOfAllRoles");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        [HttpGet("GetAllPremissionOfSpecificRoles/{Id}")]
        [SwaggerOperation(Summary = "List of All Premission Of a Specific Roles", OperationId = "List of All Premission Of  Specific Roles")]
        public async Task<IActionResult> GetAllPremissionOfSpecificRoles(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var RolePremissin = await _premissionAssignRepository.GetAllPremissionOfSpecificRoles(Id);
                if (RolePremissin != null)
                {
                    response.Data = RolePremissin;
                    response.TotalRecords = 1;
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
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionAssignControler", "GetAllPremissionOfSpecificRolesById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpDelete("DeleteRolePremission/{id}")]
        [SwaggerOperation(Summary = "delete Premission", OperationId = "Delete")]
        public async Task<IActionResult> DeleteRolePremission(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _premissionAssignRepository.DeleteRolePremission(id, Convert.ToInt32(loginUserId));
                if (deleteCategory != 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionAssignControler", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
