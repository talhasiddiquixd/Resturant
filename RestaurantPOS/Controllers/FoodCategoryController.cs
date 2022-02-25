using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RestaurantPOS.Controllers
{
    //[EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FoodCategoryController : ControllerBase
    {
        private readonly IFoodCategoryRepository _foodCategoryRepository;
        private readonly IFoodCategoryOfferRepository _foodCategoryOfferRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public FoodCategoryController(IConfiguration config, IFoodCategoryRepository foodCategoryRepository, IFoodCategoryOfferRepository foodCategoryOfferRepository, ILogger<FoodCategoryController> logger, IErrorLogRepository errorLogRepository)
        {
            _foodCategoryRepository = foodCategoryRepository;
            _foodCategoryOfferRepository = foodCategoryOfferRepository;
            _errorLogRepository = errorLogRepository;
            _config = config;
        }


        // Category List...
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all food categories", OperationId = "Get all")]
        public async Task<IActionResult> GetAl()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<FoodCategoryReponseDTO>> response = new ResponseDTO<IEnumerable<FoodCategoryReponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                var responseData = await _foodCategoryRepository.GetAll();
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
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "GetById");
            }
            return ResponseHelper<IEnumerable<FoodCategoryReponseDTO>>.GenerateResponse(response);
        }


        
        
        // Get By Id....
        [HttpGet("GetById/{id}")]
        [SwaggerOperation(Summary = "Get specific category of food", OperationId = "Get by  id")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            ResponseDTO<FoodCategoryReponseDTO> response = new ResponseDTO<FoodCategoryReponseDTO>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodCategoryRepository.GetById(id, new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                if (responseData != null)
                {
                    response.StatusCode = 200;
                    response.TotalRecords = 1;
                    response.Data = responseData;
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
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "GetById");
            }
            return ResponseHelper<FoodCategoryReponseDTO>.GenerateResponse(response);
        }


       
        
        // FoodCategory Save...
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save new category or update existing category", OperationId = "Save/Update Category")]
        [AllowAnonymous]
        public async Task<IActionResult> Post([FromBody] FoodCategoryRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.Name))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryName;
                    }
                    else if (model.AttachmentId == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryImage;
                    }
                    var alreadyExist = await _foodCategoryRepository.IsExist(model.Name);
                    if (alreadyExist == 1)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.FoodCategoryNameAlreadyEsixt;
                    }
                    if (response.IsSuccess)
                    {
                        var saveFoodCategory = await _foodCategoryRepository.Save(model, Convert.ToInt32(loginUserId));
                        if (model.IsOffer == true)
                        {
                            if (string.IsNullOrEmpty(model.OfferName))
                            {
                                response.IsSuccess = false;
                                response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryOfferName;
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
                            FoodCategoryOfferRequestDTO offerRequestDTO = new FoodCategoryOfferRequestDTO();
                            offerRequestDTO.FoodCategoryId = (int)saveFoodCategory;
                            offerRequestDTO.OfferName = model.OfferName;
                            offerRequestDTO.OfferStart = model.OfferStart;
                            offerRequestDTO.OfferEnd = model.OfferEnd;
                            offerRequestDTO.IsActive = model.IsActive;
                            var saveFoodCategoryOffer = await _foodCategoryOfferRepository.Save(offerRequestDTO);
                        }
                        var ListData = await _foodCategoryRepository.GetAll();
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
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


       
        // Delete...
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete food category by Id", OperationId = "Delete Category")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodCategoryRepository.Delete(id, Convert.ToInt32(loginUserId));
                if (responseData > 0)
                {
                    var ListData = await _foodCategoryRepository.GetAll();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.StatusCode = 404;
                    var ListData = await _foodCategoryRepository.GetAll();
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "Delete");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        
        
        // GetByFilters...
        [HttpPost("GetByFilters")]
        [SwaggerOperation(Summary = "List of all food categories custoimized details with custom filters, list address and all dishes list", OperationId = "Api for List of all cooks with custoimize details")]
        public async Task<IActionResult> GetByFilters(SearchFilter filters)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            GeneralHelper.UpdateRequestedFilters(filters, _config["GeneralConfig:Paging"]);
            try
            {
                var responseData = await _foodCategoryRepository.GetByFilters(filters);
                var dataList = responseData.FoodCategoryList;
                var dataCount = responseData.recordsCount;
                if (dataList != null)
                {
                    response.Data = dataList;
                    response.TotalRecords = dataCount;
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
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "GetByFilters");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }
    }
}