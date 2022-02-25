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
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FoodVarientController : ControllerBase
    {
        private readonly IFoodVarientRepository _foodvariantRepository;
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;
        public FoodVarientController(IFoodVarientRepository foodvaraintRepository, IConfiguration config, IErrorLogRepository errorLogRepository)
        {
            _foodvariantRepository = foodvaraintRepository;
            _config = config;
            _errorLogRepository = errorLogRepository;
        }


        // Get list of  all food varient 
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Food Varient ", OperationId = "Get all")]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<List<FoodVarientResponseDTO>> response = new ResponseDTO<List<FoodVarientResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _foodvariantRepository.GetALL();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodVarientController", "GetAll");
            }
            return ResponseHelper<List<FoodVarientResponseDTO>>.GenerateResponse(response);
        }




        //Get specific food varient by id 
        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific Food Varient  ", OperationId = "Food Varient id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _foodvariantRepository.GetById(Id);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodVarientController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        // save new food varient or update new food varient 
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Food Varient", OperationId = "Save Food Varient")]
        public async Task<IActionResult> Post([FromBody] FoodVarientRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;
                if (model.Id < 0)
                {

                    if (string.IsNullOrEmpty(model.Name))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodVariantName;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (model.FoodItemId <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodItemId;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (model.Price <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvidePrice;
                        response.StatusCode = 400;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    var IsExist = await _foodvariantRepository.IsExist(model.Name, model.FoodItemId);
                    if (!IsExist)
                    {
                        response.IsSuccess = false;
                        response.StatusCode = 400;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodVariantsName;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                }
                if (response.IsSuccess)
                {
                    SaveResult = await _foodvariantRepository.Save(model, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodVarientController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        // Delete specific food varient 
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete Food Varient", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _foodvariantRepository.Delete(id, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodVarientController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


    }
}
