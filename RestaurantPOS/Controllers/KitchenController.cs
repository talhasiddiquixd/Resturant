using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
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
    public class KitchenController : ControllerBase
    {
        private readonly IKitchenRepository _kitchenRepository;
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;

        // private readonly IJwtHelper _jwtHelpers;
        public KitchenController(IKitchenRepository kitchenRepository,IConfiguration config, IErrorLogRepository errorLogRepository)
        {
            _kitchenRepository = kitchenRepository;
            _config = config;
            _errorLogRepository = errorLogRepository;
            // _jwtHelpers = jwtHelper;
        }


        //Get list of all kitchens
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Kitchens ", OperationId = "Get all")]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<List<KitchenResponseDTO>> response = new ResponseDTO<List<KitchenResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _kitchenRepository.GetALL();
                var DataCount = data?.Count() ?? 0;
                if (DataCount > 0)
                {
                    response.TotalRecords = DataCount;
                    response.Data = data;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Critical Error: " + ex.Message;
                response.IsSuccess = false;
                response.StatusCode = 500;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenController", "GetAll");
            }
            return ResponseHelper<List<KitchenResponseDTO>>.GenerateResponse(response);
        }


        // Get kitchen by id 
        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific  Kitchen  ", OperationId = " Kitchen id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _kitchenRepository.GetById(Id);
                if (data != null)
                {
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    response.Data = data;
                    response.TotalRecords = 1;

                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        // Save new kitchen or update exesting kitchen
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Kitchen", OperationId = "Save Kitchen")]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] KitchenRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;

                if (model.Id < 0)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideKitchenId;
                    response.StatusCode = 400;
                }
                else if (string.IsNullOrEmpty(model.Name))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideKitchensName;
                    response.StatusCode = 400;
                }
                else if (string.IsNullOrEmpty(model.Description))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideKitchenDescription;
                    response.StatusCode = 400;
                }
                var IsExist = await _kitchenRepository.IsExist(model.Name);
                if (!IsExist)
                {
                    response.IsSuccess = false;
                    response.StatusCode = 400;
                    response.Message = ResponseMessageHelper.PleaseProvideKitchensName;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
                if (response.IsSuccess)
                {
                    SaveResult = await _kitchenRepository.Save(model, Convert.ToInt32(loginUserId));
                }
                if (SaveResult > 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.ActionFailed;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ResponseMessageHelper.ActionFailed;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }        


        //Delete specific kitchen by id 
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete Kitchen", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _kitchenRepository.Delete(id, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}