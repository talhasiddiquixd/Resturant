using System;
using System.Linq;
using RestaurantPOS.Helpers;
using RestaurantPOS.SignalR;
using System.Threading.Tasks;
using RestaurantPOS.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using RestaurantPOS.Helpers.UtilityHelper;
using RestaurantPOS.Helpers.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace RestaurantPOS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IHallRepository _hallRepository;
        private readonly ITableRepository _tableRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IFoodItemRepository _foodRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly IOrderNotificationsManager _orderNotificationsManager;
        private readonly IHubContext<OrderNotificationHub> _orderNotificationHubContext;
        public OrderController(ITableRepository tableRepository, IFoodItemRepository foodRepository, IHallRepository hallRepository, IOrderNotificationsManager orderNotificationsManager, IHubContext<OrderNotificationHub> orderNotificationHubContext, IConfiguration config, IOrderRepository orderRepository, IErrorLogRepository errorLogRepository)
        {
            _config = config;
            _hallRepository = hallRepository;
            _orderRepository = orderRepository;
            _foodRepository = foodRepository;
            _tableRepository = tableRepository;
            _errorLogRepository = errorLogRepository;
            _orderNotificationsManager = orderNotificationsManager;
            _orderNotificationHubContext = orderNotificationHubContext;
        }



        // Order List...
        [HttpPost("GetAll")]
        [SwaggerOperation(Summary = "Get all orders with store procedure", OperationId = "Get all with store procedure")]
        public async Task<IActionResult> GetAl(GetRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<OrdersViewModel>> response = new ResponseDTO<IEnumerable<OrdersViewModel>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                if (model.PageNo == 0 && model.PageSize == 0)
                {
                    model.PageNo = 0;
                    model.PageSize = 100;
                    model.DateFrom = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    model.DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                var responseData = await _orderRepository.GetAll(model);
                if (responseData.totalCount > 0)
                {
                    var ordersResponseData = responseData.Item1;
                    foreach (var orderResponse in ordersResponseData)
                    {
                        if(!string.IsNullOrEmpty(orderResponse.HallName))
                      orderResponse.HallName = DataEncryptionHelper.Decrypt(orderResponse.HallName, DataEncryptionHelper.EncryptionKey);
                    }
                    response.Data = ordersResponseData;
                    response.TotalRecords = responseData.totalCount;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<OrdersViewModel>>.GenerateResponse(response);
        }
        

       //  Order List...
        [HttpPost("EncryptContent")]
        [SwaggerOperation(Summary = "Get data Encrypted", OperationId = "Encrypt")]
        public async Task<IActionResult> EncryptData(string requestData)
        {
            var content = DataEncryptionHelper.Encrypt(requestData, DataEncryptionHelper.EncryptionKey ); 
           return Ok(content);
        }


        [HttpPost("GetAllwithoutstoreprocedure")]
        [SwaggerOperation(Summary = "Get all orders with out store procedure", OperationId = "Get all")]
        public async Task<IActionResult> GetAlwithoutsorprocedure(GetRequestDTO model)
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                if (model.PageNo == 0 && model.PageSize == 0)
                {
                    model.PageNo = 0;
                    model.PageSize = 100;
                    model.DateFrom = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    model.DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                var responseData = await _orderRepository.GetAllwithOutstore(model);
                if (responseData.totalCount > 0)

                {

                    var ordersResponseData = responseData.Item1;
                    foreach (var orderResponse in ordersResponseData)
                    {
                        if (!string.IsNullOrEmpty(orderResponse.HallName ))
                            orderResponse.HallName = DataEncryptionHelper.Decrypt(orderResponse.HallName, DataEncryptionHelper.EncryptionKey);
                           
                    }
                    response.TotalRecords=responseData.totalCount;
                    response.Data = responseData.Item1;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }


        // Get By Id....
        [HttpGet("GetById/{id}")]
        [SwaggerOperation(Summary = "Get specific order", OperationId = "Get by id")]
        public async Task<IActionResult> Get(int id)
        {
            ResponseDTO<OrderResponseDTO> response = new ResponseDTO<OrderResponseDTO>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.GetById(id);
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<OrderResponseDTO>.GenerateResponse(response);
        }


        //Search Order by Invoice 
        [HttpGet("SearchByInvoiceId/{Id}")]
        [SwaggerOperation(Summary = "Search orders by invoice Id", OperationId = "search by invoice id")]
        public async Task<IActionResult> SearchByinvoiceId(int Id)
        {
            ResponseDTO<IEnumerable<OrderResponseDTO>> response = new ResponseDTO<IEnumerable<OrderResponseDTO>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.SearchByInvoiceId(Id);
                if (responseData != null)
                {
                    response.TotalRecords = responseData.Count();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<OrderResponseDTO>>.GenerateResponse(response);
        }




        // GetOnGoingOrders...
        [HttpGet("GetOnGoingOrders")]
        [SwaggerOperation(Summary = "Get all on going Orders", OperationId = "Get all on going orders")]
        [AllowAnonymous]
        public IActionResult GetOnGoingOrders()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var responseData = _orderRepository.GetOnGoingOrders();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }

        [HttpGet("GetOnGoingOrdersWOstoreProcedure")]
        [SwaggerOperation(Summary = "Get all on going Orders with out store ", OperationId = "Get all on going orders")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOnGoingOrdersWOStoreProcedure()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var responseData = await _orderRepository.GetOnGoingOrdersWOStoreProcedure();
                int count = responseData.Count();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        // GetDriveThruOrders...
        [HttpGet("GetDriveThruOrders")]
        [SwaggerOperation(Summary = "Get all on going Orders", OperationId = "Get all drive thru orders")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDriveThruOrders()
        {
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            try
            {
                var responseData = _orderRepository.GetDriveThruOrders();
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }


        //get orders of specific kitchen
        [HttpGet("GetOrdersByKitchenId/{id}")]
        [SwaggerOperation(Summary = "Get All Orders of specific kitchen", OperationId = "Get Orders by kitchen id")]
        public async Task<IActionResult> GetOrdersByKitchenId(int id)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.GetOrdersByKitchenId(id);
                if (responseData != null && responseData.Count > 0)
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }



        // GetOrderByTableID
        [HttpGet("GetOrderByTableId/{tableId}")]
        [SwaggerOperation(Summary = "Get All Orders of specific kitchen", OperationId = "Get Orders by table id")]
        public async Task<IActionResult> GetOrderByTableId(int tableId)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.GetOrderByTableId(tableId);
                if (responseData != null)
                {
                    response.TotalRecords = 1;
                    response.Data = responseData;
                    response.Message = ResponseMessageHelper.DataLoadedSuccessfully;
                    ///// Send notifications to all Clients those are registered
                    var connectionIds = _orderNotificationsManager.GetUserConnections("1");
                    foreach (var connectionId in connectionIds)
                    {
                        await _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                    }
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetById");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }




        // PlaceOrder...
        [HttpPost("PlaceOrder")]
        [SwaggerOperation(Summary = "Save new order or update existing order", OperationId = "Save/Update order")]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                if (model != null)
                {
                    if (model.Amount <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideOrderAmount;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (model.TotalAmount <= 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideOrderTotalAmount;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    if (string.IsNullOrEmpty(model.CookingTime))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideCookingTime;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    else if (!model.CookingTime.Contains(":"))
                    {
                        response.IsSuccess = false;
                        response.Message = ResponseMessageHelper.PleaseProvideCookingTimeInCorrectFormate;
                        return ResponseHelper<object>.GenerateResponse(response);
                    }
                    if (response.IsSuccess)
                    {
                        if (model.TableId != null && model.TableId > 0)
                        {
                            var data = await _tableRepository.SaveStatus(model.TableId, 1);
                        }
                        
                        
                        var saveOrderId = await _orderRepository.SaveOrder(model, Convert.ToInt32(loginUserId));
                        if (saveOrderId > 0)
                        {
                            model.Id = (int)saveOrderId;
                            response.TotalRecords = 1;
                            response.StatusCode = 200;
                            response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                            var responseOrder = await _orderRepository.GetByOrderId(model.Id);
                            response.Data = responseOrder;
                            ///// Send notifications to all Clients those are registered
                            var ordertable = await _orderRepository.GetById((int)saveOrderId);
                            var connectionIds = _orderNotificationsManager.GetUserConnections("1");
                            foreach (var connectionId in connectionIds)
                            {
                                await _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("LoadOrdersNotification", response.Data);
                            }
                            var alltables = await _tableRepository.GetALL();
                            //ordertable.Tables = all tables;
                            foreach (var item in alltables)
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

                                    response.StatusCode = 500;
                                    response.Message = "Critical Error: " + ex.Message;
                                    new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                                    response.ExceptionMessage = ex.Message.ToString();
                                }
                            }
                            response.StatusCode = 200;
                            response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                            var data = await _hallRepository.GetById(ordertable.HallId);
                            ///// Send notifications to all Clients those are registered
                            connectionIds = _orderNotificationsManager.GetUserConnections("1");

                            _ = Task.Run(() =>
                              {
                                  foreach (var connectionId in connectionIds)
                                  {
                                      _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                                  }
                              });

                            // For Admin...
                            var orderTable = await _tableRepository.GetALL();
                            ordertable.Tables = orderTable;
                            foreach (var item in ordertable.Tables)
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

                                    response.StatusCode = 500;
                                    response.Message = "Critical Error: " + ex.Message;
                                    new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                                    response.ExceptionMessage = ex.Message.ToString();
                                }
                            }
                            _ = Task.Run(() =>
                            {
                                foreach (var connectionId in connectionIds)
                                {
                                    _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("AdminTableNotificationNotification", ordertable.Tables);
                                }
                            });
                            if (ordertable?.HallId != null && ordertable.HallId > 0)
                            {
                                //var data = await _hallRepository.GetById(ordertable.HallId);
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
                                }
                                foreach (var connectionId in connectionIds)
                                {
                                    await _orderNotificationHubContext.Clients.All.SendAsync("HallTableNotification", data);
                                }
                            }
                            var OngoingOrders = _orderRepository.GetOnGoingOrders();
                            _ = Task.Run(() =>
                            {
                                foreach (var connectionId in connectionIds)
                                {
                                    _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("OngoingOrderNotification", OngoingOrders);
                                }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "PlaceOrder");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        // AcceptedOrderStatus...
        [HttpPut("AcceptedOrderStatus/{OrderId}/{KitchenId}/{Status}")]
        [SwaggerOperation(Summary = "Update accepted order status by id", OperationId = "Accepted Order Status")]
        public async Task<IActionResult> AcceptedOrderStatus(int OrderId, int KitchenId, string Status)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.AcceptedOrderStatus(OrderId, KitchenId, Status);
                var responseOrder = await _orderRepository.GetOrdersByKitchenId(KitchenId);
                if (responseData > 0)
                {
                    var ChangeStatusOfOrder = await _orderRepository.UpdateOrderStatus(OrderId, Status);
                    response.TotalRecords = 1;
                    response.Data = responseOrder;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;

                    ///// Send notifications to all Clients those are registered
                    var connectionIds = _orderNotificationsManager.GetUserConnections("1");
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                        }
                    });
                    var ordertable = await _orderRepository.GetById(OrderId);
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("LoadOrdersNotification", response.Data);
                        }
                    });
                    // For Admin...
                    var alltables = await _tableRepository.GetALL();
                    ordertable.Tables = alltables;
                    foreach (var item in ordertable.Tables)
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

                            response.StatusCode = 500;
                            response.Message = "Critical Error: " + ex.Message;
                            new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                            response.ExceptionMessage = ex.Message.ToString();
                        }
                    }
                    response.Data = ordertable;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    var data = await _hallRepository.GetById(ordertable.HallId);
                    ///// Send notifications to all Clients those are registered
                    connectionIds = _orderNotificationsManager.GetUserConnections("1");
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                        }
                    });
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("AdminTableNotificationNotification", ordertable.Tables);
                        }
                    });
                    if (ordertable?.HallId != null && ordertable.HallId > 0)
                    {
                        //var data = await _hallRepository.GetById(ordertable.HallId);
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
                        }
                        _ = Task.Run(() =>
                        {
                            foreach (var connectionId in connectionIds)
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("HallTableNotification", data);
                            }
                        });
                    }
                    var OngoingOrders = _orderRepository.GetOnGoingOrders();
                    foreach (var connectionId in connectionIds)
                    {
                        await _orderNotificationHubContext.Clients.All.SendAsync("OngoingOrderNotification", OngoingOrders);
                    }
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "AcceptedOrderStatus");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        // PreparedOrderStatus...
        [HttpPut("PreparedOrderStatus/{OrderId}/{KitchenId}/{Status}")]
        [SwaggerOperation(Summary = "Update prepared order status by id", OperationId = "Prepared Order Status")]
        public async Task<IActionResult> PreparedOrderStatus(int OrderId, int KitchenId, string Status)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.PreparedOrderStatus(OrderId, KitchenId, Status);
                var responseOrder = await _orderRepository.GetOrdersByKitchenId(KitchenId);
                //var notificationMessage = "FoodItems has been prepared against Order ID: " + OrderId + " by Kitchen ID: " + Kitchen.Name; 
                var ordertable = await _orderRepository.GetById(OrderId);
                var result = await _orderRepository.OrderItemByOrderId(OrderId);
                if (result == null || result.Count <= 0)
                {
                    var PrepareOrder = await _orderRepository.UpdateOrderStatus(OrderId, "Prepared");
                }
                if (responseData > 0)
                {
                    response.TotalRecords = 1;
                    response.Data = responseOrder;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    ///// Send notifications to all Clients those are registered
                    var connectionIds = _orderNotificationsManager.GetUserConnections("1");
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                        }
                    });
                    // var connectionIds = _orderNotificationsManager.GetUserConnections("1");
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                        }
                    });
                    // For Admin...
                    var orderTable = await _tableRepository.GetALL();
                    ordertable.Tables = orderTable;
                    foreach (var item in ordertable.Tables)
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

                            response.StatusCode = 500;
                            response.Message = "Critical Error: " + ex.Message;
                            new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                            response.ExceptionMessage = ex.Message.ToString();
                        }
                    }
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("AdminTableNotificationNotification", ordertable.Tables);
                        }
                    });
                    if (ordertable?.HallId != null && ordertable.HallId > 0)
                    {
                        var data = await _hallRepository.GetById(ordertable.HallId);
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
                        }
                        _ = Task.Run(() =>
                        {
                            foreach (var connectionId in connectionIds)
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("HallTableNotification", data);
                            }
                        });
                    }
                    var OngoingOrders = _orderRepository.GetOnGoingOrders();
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("OngoingOrderNotification", OngoingOrders);
                        }
                    });
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "PreparedOrderStatus");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }




        //// UpdateOrderStatus...
        //[HttpPut("UpdateOrderStatus/{OrderId}/{Status}")]
        //[SwaggerOperation(Summary = "Update order status by id", OperationId = "Update Order Status")]
        //public async Task<IActionResult> UpdateOrderStatus(int OrderId, string Status)
        //{
        //    ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
        //    string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
        //    try
        //    {
        //        var responseData = await _orderRepository.UpdateOrderStatus(OrderId, Status);
        //        if (responseData > 0)
        //        {
        //            var ordertable = await _orderRepository.GetById(OrderId);
        //            if (Status == "Paid" || Status == "Rejected")
        //            {
        //                if (ordertable?.TableId != null && ordertable.TableId != 0)
        //                {
        //                    var FreeTable = await _tableRepository.SaveStatus(ordertable.TableId, 0);
        //                }
        //            }
        //            //if (Status == "Rejected")
        //            //{
        //            //    if (ordertable?.TableId != null && ordertable.TableId != 0)
        //            //    {
        //            //        var FreeTable = await _tableRepository.SaveStatus(ordertable.TableId, 0);
        //            //    }
        //            //}
        //            response.TotalRecords = 1;
        //            var alltables = await _tableRepository.GetALL();
        //            ordertable.Tables = alltables;
        //            foreach (var item in ordertable.Tables)
        //            {
        //                try
        //                {
        //                    if (item.IsAssigned > 0)
        //                    {
        //                        var tableOrder = await _orderRepository.GetOrderByTableId(item.Id);
        //                        item.TableOrder = tableOrder;
        //                    }
        //                }
        //                catch (Exception ex)
        //                {

        //                    response.StatusCode = 500;
        //                    response.Message = "Critical Error: " + ex.Message;
        //                    new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
        //                    response.ExceptionMessage = ex.Message.ToString();
        //                }
        //            }
        //            response.Data = ordertable;
        //            response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
        //            var data = await _hallRepository.GetById(ordertable.HallId);
        //            ///// Send notfications to all Clients those are registered
        //            var connectionIds = _orderNotificationsManager.GetUserConnections("1");
        //            _ = Task.Run(() =>
        //            {
        //                foreach (var connectionId in connectionIds)
        //                {
        //                    //if (Status == "Requested" || Status == "Preparing" || Status == "Rejected")
        //                    //{
        //                    _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
        //                    //}
        //                }
        //            });
        //            _ = Task.Run(() =>
        //            {
        //                foreach (var connectionId in connectionIds)
        //                {
        //                    _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("AdminTableNotificationNotification", ordertable.Tables);
        //                }
        //            });
        //            if (ordertable?.HallId != null && ordertable.HallId > 0)
        //            {
        //                //var data = await _hallRepository.GetById(ordertable.HallId);
        //                if (data != null)
        //                {
        //                    if (data.Table != null)
        //                    {
        //                        foreach (var Table in data.Table)
        //                        {
        //                            if (Table.IsAssigned > 0)
        //                            {
        //                                var tableOrder = await _orderRepository.GetOrderByTableId(Table.Id);
        //                                Table.TableOrder = tableOrder;
        //                            }
        //                        }
        //                    }
        //                }
        //                _ = Task.Run(() =>
        //                {
        //                    foreach (var connectionId in connectionIds)
        //                    {
        //                        _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("HallTableNotification", data);
        //                    }
        //                });
        //            }
        //            if (Status == "Paid")
        //            {
        //                //var OngoingOrders = await _orderRepository.GetOnGoingOrders(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
        //                _ = Task.Run(() =>
        //                {
        //                    foreach (var connectionId in connectionIds)
        //                    {
        //                        _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("GetPaidOrderIdNotification", OrderId);
        //                    }
        //                });
        //            }


        //        }
        //        else
        //        {
        //            response.Message = ResponseMessageHelper.NoRecordFound;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.StatusCode = 500;
        //        response.Message = "Critical Error: " + ex.Message;
        //        new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
        //        response.ExceptionMessage = ex.Message.ToString();
        //    }
        //    return ResponseHelper<object>.GenerateResponse(response);
        //}

        //UpdateOrderStatus...
        [HttpPut("UpdateOrderStatus/{OrderId}/{Status}")]
        [SwaggerOperation(Summary = "Update order status by id", OperationId = "Update Order Status")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateOrderStatus(int OrderId, string Status)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.UpdateOrderStatus(OrderId, Status);
                if (responseData > 0)
                {
                    var ordertable = await _orderRepository.GetById(OrderId);
                    if (Status == "Paid")
                    {
                        if (ordertable?.TableId != null)
                        {
                            var FreeTable = await _tableRepository.SaveStatus(ordertable.TableId, 0);
                        }
                    }
                    if (Status == "Rejected")
                    {
                        if (ordertable?.TableId != null && ordertable.TableId != 0)
                        {
                            var FreeTable = await _tableRepository.SaveStatus(ordertable.TableId, 0);
                        }
                    }
                    response.TotalRecords = 1;
                    var alltables = await _tableRepository.GetALL();
                    ordertable.Tables = alltables;
                    foreach (var item in ordertable.Tables)
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

                            response.StatusCode = 500;
                            response.Message = "Critical Error: " + ex.Message;
                            new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                            response.ExceptionMessage = ex.Message.ToString();
                        }
                    }
                    response.Data = ordertable;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    var data = await _hallRepository.GetById(ordertable.HallId);
                    ///// Send notfications to all Clients those are registered
                    var connectionIds = _orderNotificationsManager.GetUserConnections("1");
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            if (Status == "Requested" || Status == "Preparing" || Status == "Rejected")
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                            }
                        }
                    });
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("AdminTableNotificationNotification", ordertable.Tables);
                        }
                    });
                    if (ordertable?.HallId != null && ordertable.HallId > 0)
                    {
                        //var data = await _hallRepository.GetById(ordertable.HallId);
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
                        }
                        _ = Task.Run(() =>
                        {
                            foreach (var connectionId in connectionIds)
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("HallTableNotification", data);
                            }
                        });
                    }
                    if (Status == "Paid")
                    {
                        //var OngoingOrders = await _orderRepository.GetOnGoingOrders(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                        _ = Task.Run(() =>
                        {
                            foreach (var connectionId in connectionIds)
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("GetPaidOrderIdNotification", OrderId);
                            }
                        });
                    }


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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        // UpdateOrderType...
        [HttpPost("UpdateOrderType")]
        [SwaggerOperation(Summary = "Update order type by order id", OperationId = "Update order type")]
        public async Task<IActionResult> UpdateOrderType(UpdateOrderTypeRequestDTO model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var responseData = await _orderRepository.UpdateOrderType(model);
                if (model?.TableId != null && model.TableId != 0 && model.OrderType == 3)
                {
                    var FreeTable = await _tableRepository.SaveStatus(model.TableId, 1);
                }
                else if (model?.TableId != null && model.TableId != 0 && model.OrderType == 4)
                {
                    var FreeTable = await _tableRepository.SaveStatus(model.TableId, 0);
                }
                if (responseData > 0)
                {
                    response.TotalRecords = 1;
                    response.Data = model;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                }
                else
                {
                    response.Data = model;
                    response.Message = ResponseMessageHelper.NoRecordFound;
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = "Critical Error: " + ex.Message;
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        // UpdateItemStatus...
        [HttpPost("UpdateItemStatus")]
        [SwaggerOperation(Summary = "Update order item status ", OperationId = "Update Item Status")]
        public async Task<IActionResult> UpdateItemStatus(UpdateOrderItemStatus model)
        {
            ResponseDTO<object> response = new ResponseDTO<object>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            try
            {
                var ordertable = await _orderRepository.GetById(model.OrderId);
                var responseData = await _orderRepository.UpdateItemStatus(model.OrderId, model.FoodItemId, model.FoodVarientId, model.Status);
                var responseOrder = await _orderRepository.GetByOrderId(model.OrderId);
                //var result = await _orderRepository.OrderItemByOrderId(model.OrderId);
                //if (result==null)
                //{
                //    var changeOrderStatus = await _orderRepository.UpdateOrderStatus(model.OrderId, "Prepared");
                //}
                if (responseData > 0)
                {
                    response.TotalRecords = 1;
                    response.Data = responseOrder;
                    response.StatusCode = 200;
                    var alltables = await _tableRepository.GetALL();
                    ordertable.Tables = alltables;
                    foreach (var item in ordertable.Tables)
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

                            response.StatusCode = 500;
                            response.Message = "Critical Error: " + ex.Message;
                            new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateOrderStatus");
                            response.ExceptionMessage = ex.Message.ToString();
                        }
                    }
                    response.Data = ordertable;
                    response.Message = ResponseMessageHelper.ActionPerformSuccessfully;
                    var data = await _hallRepository.GetById(ordertable.HallId);
                    ///// Send notfications to all Clients those are registered
                    var connectionIds = _orderNotificationsManager.GetUserConnections("1");
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            if (model.Status == "Requested" || model.Status == "Preparing" || model.Status == "Rejected")
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("UpdateOrderStatusNotification", response.Data);
                            }
                        }
                    });
                    _ = Task.Run(() =>
                    {
                        foreach (var connectionId in connectionIds)
                        {
                            _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("AdminTableNotificationNotification", ordertable.Tables);
                        }
                    });
                    if (ordertable?.HallId != null && ordertable.HallId > 0)
                    {
                        //var data = await _hallRepository.GetById(ordertable.HallId);
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
                        }
                        _ = Task.Run(() =>
                        {
                            foreach (var connectionId in connectionIds)
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("HallTableNotification", data);
                            }
                        });
                    }
                    if (model.Status == "Paid")
                    {
                        //var OngoingOrders = await _orderRepository.GetOnGoingOrders(new ConfigurationHelper(_config, _errorLogRepository).GetBasePath("UploadFolders:Attachments"));
                        _ = Task.Run(() =>
                        {
                            foreach (var connectionId in connectionIds)
                            {
                                _orderNotificationHubContext.Clients.Client(connectionId).SendAsync("GetPaidOrderIdNotification", model.OrderId);
                            }
                        });
                    }

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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "UpdateItemStatus");
                response.ExceptionMessage = ex.Message.ToString();
            }
            return ResponseHelper<object>.GenerateResponse(response);
        }


        // GetByFilters...
        [HttpPost("GetByFilters")]
        [SwaggerOperation(Summary = "List of all orders with custom filters", OperationId = "Custom Filters")]
        public async Task<IActionResult> GetByFilters(SearchFilter filters)
        {
            ResponseDTO<IEnumerable<object>> response = new ResponseDTO<IEnumerable<object>>() { Data = null, ExceptionMessage = "", IsSuccess = true, Message = "", StatusCode = 200, TotalRecords = 0 };
            string loginUserId = User.Claims.Where(c => c.Type == "Id").Select(x => x.Value).FirstOrDefault();
            GeneralHelper.UpdateRequestedFilters(filters, _config["GeneralConfig:Paging"]);
            try
            {
                var responseData = await _orderRepository.GetByFilters(filters);
                var dataList = responseData.OrderList;
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
                new ConfigurationHelper(_config, _errorLogRepository).SaveError(ex, loginUserId, "OrderController", "GetByFilters");
            }
            return ResponseHelper<IEnumerable<object>>.GenerateResponse(response);
        }
    }
}