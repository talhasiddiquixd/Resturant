using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantPOS.Helpers;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Helpers.UtilityHelper;
using RestaurantPOS.Models;
using RestaurantPOS.Repository;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FoodItemOfferController : ControllerBase
    {
        private readonly IFoodItemOfferRepository _foodItemOfferRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public FoodItemOfferController(IConfiguration config, IFoodItemOfferRepository foodItemOfferRepository, ILogger<FoodItemOfferController> logger, IErrorLogRepository errorLogRepository)
        {
            _foodItemOfferRepository = foodItemOfferRepository;
            _errorLogRepository = errorLogRepository;
            _config = config;
        }



        
        
        // Get List all offers on foodcategory...
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all food item offers", OperationId = "Get all")]
        public async Task<IActionResult> GetAl()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<FoodItemOfferResponseDTO>> response = new ResponseDTO<IEnumerable<FoodItemOfferResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                var responseData = await _foodItemOfferRepository.GetAll();
                var count = responseData.Count();
                if (responseData != null && count > 0)
                {
                    response.StatusCode = 200;
                    response.Data = responseData;
                    response.TotalRecords = count;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemOfferController", "GetAll");
            }
            return ResponseHelper<IEnumerable<FoodItemOfferResponseDTO>>.GenerateResponse(response);
        }
        
        
        
        
        // Get specific offer By Id....
        [HttpGet("GetById/{id}")]
        [SwaggerOperation(Summary = "Get specific food item offer", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int id)
        {
            ResponseDTO<FoodItemOfferResponseDTO> response = new ResponseDTO<FoodItemOfferResponseDTO>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemOfferRepository.GetById(id);
                if (responseData != null)
                {
                    response.StatusCode = 200;
                    response.TotalRecords = 1;
                    response.Data = responseData;
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
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemOfferController", "GetById");
            }
            return ResponseHelper<FoodItemOfferResponseDTO>.GenerateResponse(response);
        }



        
        //Save new fooditem offer or update existing food item offer  ...
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save new FoodItemOffer or update existing FoodItemOffer", OperationId = "Save/Update FoodItemOffer")]
        public async Task<IActionResult> Post([FromBody] FoodItemOfferRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.OfferName))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodItemOfferName;
                    }
                    if (model.FoodItemId <= 0 )
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodItemName;
                    }
                    if (model.OfferPrice <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodItemOfferPrice;
                    }
                    if (model.OfferStart == null)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodItemOfferStartDate;
                    }
                    if (model.OfferEnd == null)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodItemOfferEndDate;
                    }
                    if (response.IsSuccess)
                    {
                        var saveFoodItemOffer = await _foodItemOfferRepository.Save(model);
                        var ListData = await _foodItemOfferRepository.GetAll();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemOfferController", "Save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        
        // InActive specific food item offer ...
        [HttpPut("InActive/{Id}/{Status}")]
        [SwaggerOperation(Summary = "InActine FoodItemOffer by Id", OperationId = "InActive FoodItemOffer")]
        public async Task<IActionResult> Delete(int Id, bool Status)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemOfferRepository.InActive(Id, Status);
                if (responseData > 0)
                {
                    var ListData = await _foodItemOfferRepository.GetAll();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    var ListData = await _foodItemOfferRepository.GetAll();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemOfferController", "Delete");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
        //// GetByFilters...
        //[HttpPost("GetByFilters")]
        //[SwaggerOperation(Summary = "List of all FoodItemOffer custoimized details with custom filters", OperationId = "Custom filters")]
        //public async Task<IActionResult> GetByFilters(SearchFilter filters)
        //{
        //    ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
        //    string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
        //    GeneralHelper.UpdateRequestedFilters(filters, _config["GeneralConfig:Paging"]);
        //    try
        //    {
        //        var responseData = await _foodItemOfferRepository.GetByFilters(filters);
        //        var dataList = responseData.FoodItemOfferList;
        //        var dataCount = responseData.recordsCount;
        //        if (dataList != null)
        //        {
        //            response.Data = dataList;
        //            response.TotalRecords = dataCount;
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
        //        response.StatusCode = 500;
        //        response.IsSuccess = false;
        //        response.ExceptionMessage = "exception" + ex;
        //        new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemOfferController", "GetByFilters");
        //    }
        //    return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        //}
    }
}