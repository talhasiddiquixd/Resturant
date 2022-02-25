using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class KitchenAssignController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IKitchenAssignRepository _kitchenAssignRepository;
        public KitchenAssignController(IKitchenAssignRepository kitchenAssignRepository,IErrorLogRepository errorLogRepository,IConfiguration config)
        {
            _config=config;
            _errorLogRepository = errorLogRepository;
            _kitchenAssignRepository = kitchenAssignRepository;
            // _jwtHelpers = jwtHelper;
        }


        
        
        
        
        
        // Get list of all assign kitchens 
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Assigned Kitchens ", OperationId = "Get all")]
       [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _kitchenAssignRepository.GetAll();
                var DataCount = data?.Count() ?? 0;
                if (DataCount > 0)
                {
                    response.Data = data;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    response.TotalRecords = DataCount;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenAssignController", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }


       
        
        
        
        
        
        // Get specific kitchen by id 
        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific Assigned Kitchen  ", OperationId = "Assigned Kitchen id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _kitchenAssignRepository.GetById(Id);
                if (data != null)
                {
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
                response.IsSuccess = false;
                response.StatusCode = 500;
                response.ExceptionMessage = "exception" + ex;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenAssignController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        
        
        
        
        
        
       
         // Assign kitchen to specific user
        [HttpPost("AssignKitchen")]
        [SwaggerOperation(Summary = "Save/Assign Kitchen Kitchen", OperationId = "Assign Kitchen")]
        public async Task<IActionResult> Post([FromBody] KitchenAssignRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;
                if (model!=null)
                {

                    if (model.KitchenId <= 0|| model?.KitchenId==null)
                    {
                        response.Message = ResponseMessageHelper.PleaseProvideKitchenId;
                        response.IsSuccess = false;
                        response.StatusCode = 400;
                    }
                    else if (model.UserId <= 0)
                    {
                        response.Message = ResponseMessageHelper.PleaseProvideUserId;
                        response.IsSuccess = false;
                        response.StatusCode = 400;
                    }
                    var IsAssigned = await _kitchenAssignRepository.AlreadyAssined(model.UserId, model.KitchenId);
                    if (IsAssigned)
                    {
                        response.Message = ResponseMessageHelper.AKitchenHasbeenAlreadyAssignToThisUser;
                        response.IsSuccess = false;
                        response.StatusCode = 200;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    if (response.IsSuccess)
                    {
                        SaveResult = await _kitchenAssignRepository.Save(model);
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
                else
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PostedDataIsNotValid;
                    response.StatusCode = 400;
                    return ResponseHelper<object>.GenerateResponse(response);

                }
            }
            catch (Exception ex)
            {
                response.Message = ResponseMessageHelper.ActionFailed;
                response.IsSuccess = false;
                response.StatusCode = 500;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenAssignController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        
        
        
        
        
        
        
        // Delete assign kitchen 
        [HttpDelete("Delete/{Id}")]
        [SwaggerOperation(Summary = "Delete Assigned Kitchen", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _kitchenAssignRepository.Delete(Id);
                if (deleteCategory != 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.Message = ResponseMessageHelper.ActionFailed;
                response.IsSuccess = false;
                response.StatusCode = 400;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "KitchenAssignController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
