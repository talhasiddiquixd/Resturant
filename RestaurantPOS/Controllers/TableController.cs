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
   // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TableController : ControllerBase
    {
        private readonly ITableRepository _tableRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IConfiguration _config;
        public TableController(ITableRepository tableRepository, IConfiguration config, IErrorLogRepository errorLogRepository, IOrderRepository orderRepository)
        {
            _tableRepository = tableRepository;
            _orderRepository = orderRepository;
            _errorLogRepository = errorLogRepository;
            _config = config;
        }


        [HttpGet("GetAll")]
        [SwaggerOperation(Summary = "List of all Tables ", OperationId = "Get all")]
        public async Task<IActionResult> GetAll()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<List<TableResponseDTO>> response = new ResponseDTO<List<TableResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _tableRepository.GetALL();
                foreach (var item in data)
                {
                    try
                    {
                        if (item.IsAssigned > 0)
                        {
                            var tableOrder = await _orderRepository.GetOrderByTableId(item.Id);
                            item.TableOrder = tableOrder;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = "Critical Error: " + ex.Message;
                        response.IsSuccess = false;
                        response.StatusCode = 500;
                        new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "TableControler", "GetAll");
                        response.ExceptionMessage = ex.Message.ToString();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "TableControler", "GetAll");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<List<TableResponseDTO>>.GenerateResponse(response);
        }



        [HttpGet("GetAllFreeTable")]
        [SwaggerOperation(Summary = "List of all frer Tables ", OperationId = "Get free all")]
        public async Task<IActionResult> GetAllFreeTable()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<List<TableResponseDTO>> response = new ResponseDTO<List<TableResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var data = await _tableRepository.GetALLFreeTable();
                var DataCount = data?.Count() ?? 0;
                if (DataCount > 0)
                {
                    foreach (var item in data)
                    {
                        if (item.IsAssigned > 0)
                        {
                            var tableOrder = await _orderRepository.GetOrderByTableId(item.Id);
                            item.TableOrder = tableOrder;
                        }
                    }
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "TableControler", "GetAllFreeTable");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<List<TableResponseDTO>>.GenerateResponse(response);
        }



        [HttpGet("GetByid/{Id}")]
        [SwaggerOperation(Summary = "Get specific  Table  ", OperationId = "Get by table id")]
        public async Task<IActionResult> GetById(int Id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var data = await _tableRepository.GetById(Id);
                if (data.IsAssigned > 0)
                {
                    var tableOrder = await _orderRepository.GetOrderByTableId(data.Id);
                    data.TableOrder = tableOrder;
                }
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "TableControler", "GetByid");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpPost("Save")]
        [SwaggerOperation(Summary = "Save/Update Table", OperationId = "Save Table")]
        public async Task<IActionResult> Post([FromBody] TableRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;

                if (model.Id < 0)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideTableId;
                    response.StatusCode = 400;
                }
                else if (string.IsNullOrEmpty(model.Name))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideTableName;
                    response.StatusCode = 400;
                }
                else if (string.IsNullOrEmpty(model.Description))
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideTableDescription;
                    response.StatusCode = 400;
                }
                var IsExist = await _tableRepository.IsExist(model.Name);
                if (!IsExist)
                {
                    response.IsSuccess = false;
                    response.StatusCode = 400;
                    response.Message = ResponseMessageHelper.PleaseProvideTablesName;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
                if (response.IsSuccess)
                {
                    SaveResult = await _tableRepository.Save(model, Convert.ToInt32(loginUserId));
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "TableControler", "Save");
                response.Message = ResponseMessageHelper.ActionFailed;
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpPut("UnassignedTable")]
        [SwaggerOperation(Summary = "Unassigned Table", OperationId = "UnAssigned Table")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Put([FromForm] UnassignedTableRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                long SaveResult = 0;

                if (model.TableId < 0)
                {
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.PleaseProvideTableId;
                    response.StatusCode = 400;
                }
                if (response.IsSuccess)
                {
                    SaveResult = await _tableRepository.SaveStatus(model.TableId, 0);
                }
                if (SaveResult > 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    return ResponseHelper<object>.GenerateResponse(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "TableControler", "Save");
                response.Message = ResponseMessageHelper.ActionFailed;
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }



        [HttpDelete("Delete/{id}")]
        [SwaggerOperation(Summary = "Delete Table", OperationId = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var deleteCategory = await _tableRepository.Delete(id, Convert.ToInt32(loginUserId));
                if (deleteCategory != 0)
                {
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.StatusCode = 400;
                    response.IsSuccess = false;
                    response.Message = ResponseMessageHelper.ActionFailed;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.IsSuccess = false;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, "0", "TableControler", "Save");
                response.Message = ResponseMessageHelper.ActionFailed;
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


    }
}
