using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Helpers.UtilityHelper;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionRepository _premissionRepository;
        // private readonly ILogger<PermissionController> _logger;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public PermissionController(IPermissionRepository premissionRepository, IConfiguration config, IErrorLogRepository errorLogRepository)//, ILogger<PermissionController> logger)
        {
            //_logger = logger;
            _config = config;
            _premissionRepository = premissionRepository;
            _errorLogRepository = errorLogRepository;
        }


        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of Premission", OperationId = "Premission List")]
        public async Task<IActionResult> Get()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var CookData = await _premissionRepository.Get();
                var CountCook = CookData?.Count() ?? 0;
                if (CookData != null)
                {
                    response.Data = CookData;
                    response.TotalRecords = CountCook;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionControler", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        [HttpGet("GetById/{PremissionId}")]
        [SwaggerOperation(Summary = "Get specific Premission", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int PremissionId)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _premissionRepository.Get(PremissionId);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "PermissionController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Premission", OperationId = "Save Premission")]
        public async Task<IActionResult> Post([FromBody] PermissionRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrEmpty(model.Name))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvidePremissionName;
                }
                if (model != null && response.IsSuccess)
                {
                    SaveResult = await _premissionRepository.Save(model, Convert.ToInt32(loginUserId));
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
                response.IsSuccess = false;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionController", "save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete Premission", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _premissionRepository.Delete(id, Convert.ToInt32(loginUserId));
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
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "PermissionController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
       

    }
}

