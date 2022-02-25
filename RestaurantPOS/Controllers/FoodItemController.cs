using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
using Microsoft.AspNetCore.Cors;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RestaurantPOS.Controllers
{
    //[EnableCors("AllowOrigin")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FoodItemController : ControllerBase
    {
        private readonly IFoodItemRepository _foodItemRepository;
        private readonly IFoodItemOfferRepository _foodItemOfferRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public FoodItemController(IConfiguration config, IFoodItemRepository foodItemRepository, IFoodItemOfferRepository foodItemOfferRepository, ILogger<FoodItemController> logger, IErrorLogRepository errorLogRepository)
        {
            _foodItemRepository = foodItemRepository;
            _foodItemOfferRepository = foodItemOfferRepository;
            _errorLogRepository = errorLogRepository;
            _config = config;
        }




        // Get List of all food catregories...
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "Get all food items", OperationId = "Get all")]
        public async Task<IActionResult> GetAl()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<FoodItemResponseDTO>> response = new ResponseDTO<IEnumerable<FoodItemResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var responseData = await _foodItemRepository.GetAll(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                var count = responseData.Count();
                if (responseData != null && count > 0)
                {
                    response.Data = responseData;
                    response.TotalRecords = count;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "GetById");
            }
            return ResponseHelper<IEnumerable<FoodItemResponseDTO>>.GenerateResponse(response);
        }




        // Get specific food item By Id....
        [HttpGet("GetById/{id}")]
        [SwaggerOperation(Summary = "Get specific food item", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int id)
        {
            ResponseDTO<FoodItemResponseDTO> response = new ResponseDTO<FoodItemResponseDTO>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemRepository.GetById(id, new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                if (responseData != null)
                {
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "GetById");
            }
            return ResponseHelper<FoodItemResponseDTO>.GenerateResponse(response);
        }



        // Search  food item By Id Or Name....
        [HttpPost("SearchFoodItem")]
        [SwaggerOperation(Summary = "Get specific food item", OperationId = "Search Food Item")]
        public async Task<IActionResult> SearchFoodItem(SearchFoodItemRequestDTO model)
        {
            ResponseDTO<IEnumerable<FoodItemResponseDTO>> response = new ResponseDTO<IEnumerable<FoodItemResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemRepository.SearchFoodItem(model);
                if (responseData != null)
                {
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "SearchFoodItem");
            }
            return ResponseHelper<IEnumerable<FoodItemResponseDTO>>.GenerateResponse(response);
        }




        // Search  food item By Id Or Name....
        [HttpPost("SearchFoodItemUpDated")]
        [SwaggerOperation(Summary = "Get specific food item", OperationId = "Search Food Item new")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchFoodItemUpDate(SearchFoodItemRequestDTO model)
        {
            ResponseDTO<IEnumerable<SearchFoodItemResponseDTO>> response = new ResponseDTO<IEnumerable<SearchFoodItemResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemRepository.SearchFoodItemUpd(model);
                if (responseData != null)
                {
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "SearchFoodItem");
            }
            return ResponseHelper<IEnumerable<SearchFoodItemResponseDTO>>.GenerateResponse(response);
        }





        //Get all food item with varient
        [HttpGet("GetAllFooditemWithvariant")]
        [SwaggerOperation(Summary = "Get All Food item With variant", OperationId = "get Food Item new")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFooditemWithvariant()
        {
            ResponseDTO<IEnumerable<SearchFoodItemResponseDTO>> response = new ResponseDTO<IEnumerable<SearchFoodItemResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemRepository.GetAllFooditemWithvariant();
                if (responseData != null)
                {
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "SearchFoodItem");
            }
            return ResponseHelper<IEnumerable<SearchFoodItemResponseDTO>>.GenerateResponse(response);
        }




        //Save new FoodCategory or update existing food item...
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save new food item or update existing food item", OperationId = "Save/Update food item")]
        public async Task<IActionResult> Post([FromBody] FoodItemRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                if (model != null)
                {
                    if (string.IsNullOrEmpty(model.Name))
                    {
                        response.IsSuccess = false;
                        response.StatusCode = 400;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodItemName;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (model.FoodCategoryId <= 0)
                    {
                        response.IsSuccess = false;
                        response.StatusCode = 400;
                        response.Message = ResponseMessageHelper.PleaseProvideFoodCategoryName;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (model.KitchenId <= 0)
                    {
                        response.IsSuccess = false;
                        response.StatusCode = 400;
                        response.Message = ResponseMessageHelper.PleaseProvideKitchenName;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    var alreadyExist = await _foodItemRepository.IsExist(model.Name);
                    if (alreadyExist == 1)
                    {
                        response.StatusCode = 400;
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.FoodItemNameAlreadyEsixt;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    if (response.IsSuccess)
                    {
                        var time = model.CookingTime;
                        model.CookingTime = "00:" + time;
                        var saveFoodItem = await _foodItemRepository.Save(model, Convert.ToInt32(loginUserId));
                        if (model.IsOffer == true)
                        {
                            //model.FoodCategoryId = (int)saveFoodCategory;
                            FoodItemOfferRequestDTO itemOfferRequestDTO = new FoodItemOfferRequestDTO();
                            itemOfferRequestDTO.FoodItemId = (int)saveFoodItem;
                            itemOfferRequestDTO.OfferName = model.OfferName;
                            itemOfferRequestDTO.OfferStart = model.OfferStart;
                            itemOfferRequestDTO.OfferEnd = model.OfferEnd;
                            itemOfferRequestDTO.IsActive = model.IsActive;
                            var saveFoodItemOffer = await _foodItemOfferRepository.Save(itemOfferRequestDTO);
                            if (saveFoodItem > 0)
                            {
                                var ListData = await _foodItemRepository.GetAll(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                                response.Data = model;
                                response.TotalRecords = ListData.Count();
                                response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                                return ResponseHelper<object>.GenerateResponse(response);
                            }
                        }
                        if (saveFoodItem > 0)
                        {
                            var ListData = await _foodItemRepository.GetAll(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                            response.Data = model;
                            response.TotalRecords = ListData.Count();
                            response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                            return ResponseHelper<object>.GenerateResponse(response);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "Save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        // Delete specific food item...
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "InActive  food item by Id", OperationId = "InActive food item")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemRepository.Delete(id);
                if (responseData > 0)
                {
                    var ListData = await _foodItemRepository.GetAll(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "Delete");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        [HttpGet("ActiveFoodItem/{id}")]
        [SwaggerOperation(Summary = "Active food item by Id", OperationId = "Active food item")]
        public async Task<IActionResult> ActiveFoodItem(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _foodItemRepository.ActiveFoodItem(id);
                if (responseData > 0)
                {
                    var ListData = await _foodItemRepository.GetAll(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                    response.TotalRecords = ListData.Count();
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "Delete");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        // GetByFilters food item by custom filter...
        [HttpPost("GetByFilters")]
        [SwaggerOperation(Summary = "List of all food item with custom filters", OperationId = "Custom Filters")]
        public async Task<IActionResult> GetByFilters(SearchFilter filters)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            GeneralHelper.UpdateRequestedFilters(filters, _config["GeneralConfig:Paging"]);
            try
            {
                var responseData = await _foodItemRepository.GetByFilters(filters);
                var dataList = responseData.FoodItemList;
                var dataCount = responseData.recordsCount;
                if (dataList != null)
                {
                    response.Data = dataList;
                    response.TotalRecords = dataCount;
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
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodItemController", "GetByFilters");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }


    }
}