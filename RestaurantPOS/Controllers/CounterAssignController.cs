using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Helpers.UtilityHelper;
using RestaurantPOS.Repository;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[EnableCors("AllowOrigin")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CounterAssignController : ControllerBase
    {
        private readonly ICounterAssignRepository _counterAssignRepository;
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;
        public CounterAssignController(ICounterAssignRepository counterAssignRepository, IConfiguration config, IErrorLogRepository errorLogRepository)
        {
            _counterAssignRepository = counterAssignRepository;
            _config = config;
            _errorLogRepository = errorLogRepository;
        }

        //Get All Cunter Assign
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Assigned Counter", OperationId = "Get all")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _counterAssignRepository.GetALL();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterAssignController", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }

        
        //Get Counter  specific assign 
        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific Assigned Counter", OperationId = "Assigned Counter id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _counterAssignRepository.GetById(Id);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterAssignController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }

        
        //Save or Update Assign Counter 
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Assign Counter", OperationId = "Save Assign Counter")]
        public async Task<IActionResult> Post([FromBody] CounterAssignRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;
                if (model!=null)
                {
                    if (model.Id < 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideCounterAssignId;
                        response.StatusCode = 400;
                    }
                    else if (model.CounterId < 0|| model?.CounterId == null)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideCounterId;
                        response.StatusCode = 400;
                    }
                    else if (model.UserId < 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideUserId;
                        response.StatusCode = 400;
                    }
                    //else if (string.IsNullOrEmpty(model.Name))
                    //{
                    //    response.IsSuccess = false;
                    //    response.Message = ResponseMessageHelper.PleaseProvideCounterAssignName;
                    //    response.StatusCode = 400;
                    //}
                    //var IsExist = await _counterAssignRepository.IsExist(model.Name);
                    //if (!IsExist)
                    //{
                    //    response.IsSuccess = false;
                    //    response.Message = ResponseMessageHelper.PleaseProvideCounterAssignsName;
                    //    return ResponseHelper<object>.GenerateResponse(response);
                    //}
                    var IsAssigned = await _counterAssignRepository.AlreadyAssined(model.UserId, model.CounterId);
                    if (IsAssigned)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.ThisCounterHasbeenAlreadyAssignToThisUser;
                        response.StatusCode = 200;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    if (response.IsSuccess)
                    {
                        SaveResult = await _counterAssignRepository.Save(model, Convert.ToInt32(loginUserId));
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
                response.StatusCode = 500;
                response.IsSuccess = false;
                response.Message = ResponseMessageHelper.ActionFailed;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterAssignController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }

        
        
        
        
        
        //Delete Assign Counter
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete AssignedCounter", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _counterAssignRepository.Delete(id, Convert.ToInt32(loginUserId));
                if (deleteCategory != 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch(Exception ex)
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterAssignController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
