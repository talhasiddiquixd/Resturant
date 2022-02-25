using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Repository;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RestaurantPOS.Controllers
{
    //[EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FoodCategoryOfferController : ControllerBase
    {
        private readonly IFoodCategoryOfferRepository _foodCategoryOfferRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public FoodCategoryOfferController(IConfiguration config, IFoodCategoryOfferRepository foodCategoryOfferRepository, ILogger<FoodCategoryOfferController> logger, IErrorLogRepository errorLogRepository)
        {
            _foodCategoryOfferRepository = foodCategoryOfferRepository;
            _errorLogRepository = errorLogRepository;
            _config = config;
        }


        
        
        // Category List...
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all food category offers", OperationId = "Get all")]
        public async Task<IActionResult> GetAl()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<FoodCategoryOfferResponseDTO>> response = new ResponseDTO<IEnumerable<FoodCategoryOfferResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                var responseData = await _foodCategoryOfferRepository.GetAll();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryOfferController", "GetAll");
            }
            return ResponseHelper<IEnumerable<FoodCategoryOfferResponseDTO>>.GenerateResponse(response);
        }



        
        
        // Get By Id....
        [HttpGet("GetById/{id}")]
        [SwaggerOperation(Summary = "Get specific food category offer", OperationId = "Get by  id")]
        public async Task<IActionResult> Get(int id)
        {
            ResponseDTO<FoodCategoryOfferResponseDTO> response = new ResponseDTO<FoodCategoryOfferResponseDTO>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodCategoryOfferRepository.GetById(id);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryOfferController", "GetById");
            }
            return ResponseHelper<FoodCategoryOfferResponseDTO>.GenerateResponse(response);
        }



        
        
        // FoodCategory Save...
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save new FoodCategoryOffer or update existing FoodCategoryOffer", OperationId = "Save/Update FoodCategoryOffer")]
        public async Task<IActionResult> Post([FromBody] FoodCategoryOfferRequestDTO model)
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
                        response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryOfferName;

                    }
                    if (model.FoodCategoryId <= 0 )
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryName;

                    }
                    if (model.OfferStart == null)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryOfferStartDate;

                    }
                    if (model.OfferEnd == null)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryOfferEndDate;

                    }
                    if (response.IsSuccess)
                    {
                        var saveFoodCategoryOffer = await _foodCategoryOfferRepository.Save(model);
                        var ListData = await _foodCategoryOfferRepository.GetAll();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryOfferController", "Save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        
        
        // InActive food categoryOffer...
        [HttpPut("InActive/{Id}/{Status}")]
        [SwaggerOperation(Summary = "InActine FoodCategoryOffer by Id", OperationId = "InActive FoodCategoryOffer")]
        public async Task<IActionResult> Delete(int Id, bool Status)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodCategoryOfferRepository.InActive(Id, Status);
                if (responseData > 0)
                {
                    var ListData = await _foodCategoryOfferRepository.GetAll();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    var ListData = await _foodCategoryOfferRepository.GetAll();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryOfferController", "Delete");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        //// GetByFilters...
        //[HttpPost("GetByFilters")]
        //[SwaggerOperation(Summary = "List of all FoodCategoryOffer custoimized details with custom filters", OperationId = "Custom filters")]
        //public async Task<IActionResult> GetByFilters(SearchFilter filters)
        //{
        //    ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
        //    string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
        //    GeneralHelper.UpdateRequestedFilters(filters, _config["GeneralConfig:Paging"]);
        //    try
        //    {
        //        var responseData = await _foodCategoryOfferRepository.GetByFilters(filters);
        //        var dataList = responseData.FoodCategoryOfferList;
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
        //        new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryOfferController", "GetByFilters");
        //    }
        //    return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        //}
    }
}