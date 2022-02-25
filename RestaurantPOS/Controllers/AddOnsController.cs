using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors;

namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AddOnsController : ControllerBase
    {
        private readonly IAddOnsRepository _addOnsRepository;
        // private readonly ILogger<PermissionController> _logger;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public AddOnsController(IAddOnsRepository addOnsRepository, IConfiguration config, IErrorLogRepository errorLogRepository)//, ILogger<PermissionController> logger)
        {
            //_logger = logger;
            _config = config;
            _addOnsRepository = addOnsRepository;
            _errorLogRepository = errorLogRepository;
        }
        ////GetAll...
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of AddOns Updated", OperationId = "AddOns List Updated")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var AddOnsData = await _addOnsRepository.Get();
                var AddOnscuuntData = AddOnsData?.Count() ?? 0;
                if (AddOnsData != null && AddOnscuuntData>0)
                {
                    response.Data = AddOnsData;
                    response.TotalRecords = AddOnscuuntData;
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
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserControler", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }
        
        
        
        ////GetById/{AddOnsId}...
        [HttpGet("GetById/{AddOnsId}")]
        [SwaggerOperation(Summary = "Get specific AddOns", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int AddOnsId)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() 
            { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _addOnsRepository.Get(AddOnsId);
                if (responseData != null)
                {
                    response.StatusCode = 200;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
       
        
        
        ////Save...
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update AddOns", OperationId = "Save AdOns")]
        public async Task<IActionResult> Post([FromBody] AddOnsRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrEmpty(model.Name))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideAddOnsName;
                }
                if (model != null && response.IsSuccess)
                {
                    SaveResult = await _addOnsRepository.Save(model, Convert.ToInt32(loginUserId));
                }
                if (SaveResult >= 0)
                {
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                response.IsSuccess = false;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
       
        
        
        
        
        ////AssignAddOns...
        [HttpPost("AssignAddOns")]
        [SwaggerOperation(Summary = "Assign Add ons food varients", OperationId = "Assign  Add on to food varients")]
        public async Task<IActionResult> AssignPremissions([FromBody] AssignAddOnsRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            try
            {
                long SaveResult = 0;
                if (model.AddOnsId <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideAddOnsName;
                }
                else if (model.FoodVarientId <= 0)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideFoodVarient;
                }
                if (model != null && response.IsSuccess)
                {
                    SaveResult = await _addOnsRepository.AssignAddOns(model, Convert.ToInt32(loginUserId));
                }
                if (SaveResult >= 0)
                {
                    response.StatusCode = 200;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ResponseMessageHelper.ActionFailed;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserController", "save");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
        
        
        
        
        ////GetAllAssignAddOns...
        [HttpGet("GetAllAssignAddOns")]
        [SwaggerOperation(Summary = "List of Assign AddOns Updated", OperationId = "Assign AddOns List Updated")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAssignAddOns()
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var AddOnsData = await _addOnsRepository.GetAssignAddOns();
                var AddOnscuuntData = AddOnsData?.Count() ?? 0;
                if (AddOnsData != null && AddOnscuuntData > 0)
                {
                    response.Data = AddOnsData;
                    response.TotalRecords = AddOnscuuntData;
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
                response.StatusCode = 404;
                response.IsSuccess = false;
                response.ExceptionMessage = "exception" + ex;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "UserControler", "GetAll");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }
        
        
        
        ////Get  AddOnsAssign byId}...
        [HttpGet("GetByAddOnsAssignId/{Id}")]
        [SwaggerOperation(Summary = "Get specific AddOnsAssign Id", OperationId = "Get by Assign AddOns Id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 400, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _addOnsRepository.GetById(Id);
                if (responseData != null)
                {
                    response.StatusCode = 200;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "FoodCategoryController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
        
        
        
        
        
        ////Delete  AddOnsAssign ...
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete addOns", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _addOnsRepository.Delete(id, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "AddOnsController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
