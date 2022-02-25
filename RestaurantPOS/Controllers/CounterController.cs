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
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace RestaurantPOS.Controllers
{
    //[EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CounterController : ControllerBase
    {
        private readonly ICounterRepository _counterRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public CounterController(ICounterRepository counterRepository, IConfiguration config, IErrorLogRepository errorLogRepository)
        {
            _counterRepository = counterRepository;
            _config = config;
            _errorLogRepository = errorLogRepository;
        }

        // Get list of  all counter
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Counter", OperationId = "Get all")]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<List<CounterResponseDTO>> response = new ResponseDTO<List<CounterResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _counterRepository.GetALL();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterController", "GetAll");
            }
            return ResponseHelper<List<CounterResponseDTO>>.GenerateResponse(response);
        }

        
        
        //Get specific counter 
        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific Counter", OperationId = " Counter id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _counterRepository.GetById(Id);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }

        
        
        // save new counter or update existing counter 
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Counter", OperationId = "Save Counter")]
        public async Task<IActionResult> Post([FromBody] CounterRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;

                if (model.Id < 0)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideCounterId;
                    response.StatusCode = 400;
                }
                else if (string.IsNullOrEmpty(model.Name))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideCounterName;
                    response.StatusCode = 400;
                }                
                var IsExist = await _counterRepository.IsExist(model.Name);
                if (!IsExist)
                {
                    response.IsSuccess = false;
                    response.StatusCode = 400;
                    response.Message = ResponseMessageHelper.PleaseProvideCountersName;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
                if (response.IsSuccess)
                {
                    SaveResult = await _counterRepository.Save(model, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }

      
        
        
        
       
        
        // Delete a specific counter 
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete Counter", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _counterRepository.Delete(id, Convert.ToInt32(loginUserId));
                if (deleteCategory != 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 400;
                response.IsSuccess = false;
                response.ExceptionMessage = ex.Message.ToString();
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "CounterController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
