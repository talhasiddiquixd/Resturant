using System;
using System.Linq;
using RestaurantPOS.Helpers;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using RestaurantPOS.Helpers.UtilityHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;

namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class HallController : ControllerBase
    {
        private readonly IHallRepository _hallRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IConfiguration _config;
        private readonly IErrorLogRepository _errorLogRepository;
        public HallController(IHallRepository hallRepository, IOrderRepository orderRepository, IErrorLogRepository errorLogRepository, IConfiguration config)
        {
            _config = config;
            _errorLogRepository = errorLogRepository;
            _hallRepository = hallRepository;
            _orderRepository = orderRepository;
        }



        //Get list of all halls with tables
        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Halls ", OperationId = "Get all")]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<List<HallResponseDTO>> response = new ResponseDTO<List<HallResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _hallRepository.GetALL();
                foreach (var item in data)
                {
                    if (item.Table != null)
                    {
                        foreach (var Table in item.Table)
                        {
                            if (Table.IsAssigned>0)
                            {
                                var tableOrder = await _orderRepository.GetOrderByTableId(Table.Id);
                                Table.TableOrder = tableOrder; 
                            }
                        }
                    }
                }
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallController", "GetAll");
            }
            return ResponseHelper<List<HallResponseDTO>>.GenerateResponse(response);
        }


        
        //Get specific hall by id 
        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific  Hall  ", OperationId = " Hall id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _hallRepository.GetById(Id);
                if (data != null)
                {
                    if (data.Table != null)
                    {
                        foreach (var Table in data.Table)
                        {
                            if (Table.IsAssigned > 0)
                            {
                                var tableOrder = await _orderRepository.GetOrderByTableId(Table.Id);
                                Table.TableOrder = tableOrder;
                            }
                        }
                    }
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
                response.IsSuccess = false;
                response.StatusCode = 500;
                response.ExceptionMessage = "exception" + ex;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallController", "GetById");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


       
        
        //Save new hall or update exesting hall
        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Hall", OperationId = "Save Hall")]
        public async Task<IActionResult> Post([FromBody] HallRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;

                if (model.Id < 0)
                {
                    response.Message = ResponseMessageHelper.PleaseProvideHallId;
                    response.IsSuccess = false;
                    response.StatusCode = 400;
                }
                else if (string.IsNullOrEmpty(model.Name))
                {
                    response.Message = ResponseMessageHelper.PleaseProvideHallName;
                    response.IsSuccess = false;
                    response.StatusCode = 400;
                }
                else if (string.IsNullOrEmpty(model.Description))
                {
                    response.Message = ResponseMessageHelper.PleaseProvideHallDescription;
                    response.IsSuccess = false;
                    response.StatusCode = 400;
                }
                else if (model.Id <= 0)
                {
                    var IsExist = await _hallRepository.IsExist(model.Name);
                    if (!IsExist)
                    {
                        response.Message = ResponseMessageHelper.PleaseProvideHallsName;
                        response.IsSuccess = false;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                }
                if (response.IsSuccess)
                {
                    SaveResult = await _hallRepository.Save(model, Convert.ToInt32(loginUserId));
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
                response.Message = ResponseMessageHelper.ActionFailed;
                response.IsSuccess = false;
                response.StatusCode = 500;
                response.ExceptionMessage = ex.Message.ToString();
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallController", "Save");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        
        
        // Delete hall by id 
        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete Hall", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _hallRepository.Delete(id, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "HallController", "Delete");
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }
    }
}
