using System;
using System.Linq;
using System.Threading.Tasks;
using RestaurantPOS.Helpers;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Repository;
using System.Collections.Generic;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public RestaurantController(IRestaurantRepository restaurantRepository, IConfiguration config, IErrorLogRepository errorLogRepository)//, ILogger<PermissionController> logger)
        {
            //_logger = logger;
            _config = config;
            _restaurantRepository = restaurantRepository;
            _errorLogRepository = errorLogRepository;
        }


       
        
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of restuarant settings", OperationId = "setting list")]
        public async Task<IActionResult> Get()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var AddOnsData = await _restaurantRepository.Get(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                var AddOnscuuntData = AddOnsData?.Count() ?? 0;
                if (AddOnsData != null && AddOnscuuntData > 0)
                {
                    response.Data = AddOnsData;
                    response.TotalRecords = AddOnscuuntData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "RestaurantControler", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        
        
        
        [HttpGet("GetById/{settingId}")]
        [SwaggerOperation(Summary = "Get specific retaurant setting", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int settingId)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _restaurantRepository.Get(settingId, new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "RestaurantController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }





        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update restaurant setting", OperationId = "Save restaurant setting")]
        public async Task<IActionResult> Post([FromBody] RestaurantRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                if (model.Id>0)
                {
                    if (string.IsNullOrEmpty(model.RestaurantName))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideReastaurantName;
                    }
                    if (string.IsNullOrEmpty(model.Address))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideReastaurantAddress;
                    }
                    if (string.IsNullOrEmpty(model.ContactNo))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideReastaurantContactNo;
                    } 
                }
                if (model != null && response.IsSuccess)
                {
                    SaveResult = await _restaurantRepository.Save(model, Convert.ToInt32(loginUserId));
                }
                if (SaveResult >= 0)
                {
                    response.StatusCode = 200;
                    response.Data = model;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                response.IsSuccess = false;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "RestaurantController", "Save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }

        
        
        
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete restaurant setting", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _restaurantRepository.Delete(id);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "RestaurantController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
