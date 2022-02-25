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
    public class HallAssignController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IHallAssignRepository _hallAssignRepository;
        public HallAssignController(IHallAssignRepository hallAssignRepository, IConfiguration config, IErrorLogRepository errorLogRepository)
        {
            _config = config;
            _errorLogRepository = errorLogRepository;
            _hallAssignRepository = hallAssignRepository;
        }


        
        
        // get list of all assigned halls 
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Assigned Halls ", OperationId = "Get all")]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _hallAssignRepository.GetALL();
                var DataCount = data?.Count() ?? 0;
                if (DataCount > 0)
                {
                    response.Data = data;
                    response.TotalRecords = DataCount;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallAssignController", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }


       
        
        // Get hall asign by id 
        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific Assigned Hall  ", OperationId = "Assigned Hall id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _hallAssignRepository.GetById(Id);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallAssignController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        
        
        //save new assign hall or update new assign hall
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Assigned Hall", OperationId = "Save Assigned Hall")]
        public async Task<IActionResult> Post([FromBody] HallAssignRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;
                if (model != null)
                {
                    if (model.Id < 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideHallAssignId;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (model.UserId <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideUserId;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (model.HallId <= 0|| model?.HallId==null)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideHallId;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    var IsAssigned = await _hallAssignRepository.AlreadyAssined(model.UserId, model.HallId);
                    if (IsAssigned)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.AHallHasbeenAlreadyAssignToThisUser;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    if (response.IsSuccess)
                    {
                        SaveResult = await _hallAssignRepository.Save(model, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallAssignController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        
        //Delete specific hall assign
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete Assigned Hall", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _hallAssignRepository.Delete(id, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallAssignController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}

