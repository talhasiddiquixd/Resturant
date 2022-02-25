using System;
using LinqKit;
using AutoMapper;
using System.Linq;
using RestaurantPOS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
using RestaurantPOS.Helpers.ViewModels;
namespace RestaurantPOS.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IunitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<Order>> GetALLSync()
        {
            var dataList = await _unitOfWork.Context.Order.Where(x => x.IsSynchronized == false).ToListAsync();
            return dataList;
        }
        public async Task<(IEnumerable<OrdersViewModel>, int totalCount)> GetAll(GetRequestDTO model)
        {
            string orderStatus = "Requested_Preparing_Prepared_Served_Paid";
            var totalCount = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected").CountAsync();
            var OrderResponseList = _unitOfWork.Context.OrdersViewModel.FromSqlRaw($"EXEC sp_GetAllOrders @orderStatus='{orderStatus}',@pageNo='{model.PageNo}',@pageSize='{model.PageSize}',@dateFrom='{model.DateFrom}',@dateTo='{model.DateTo}'").AsEnumerable().ToList(); ///.Where(x => x.OrderStatus != "Rejected").Include(x => x.OrderItem).ToListAsync();
            var allOrderItems = _unitOfWork.Context.OrderItemsViewModel.FromSqlRaw($"EXEC sp_GetAllOrderItems @orderStatus='{orderStatus}',@pageNo='{model.PageNo}',@pageSize='{model.PageSize}',@dateFrom='{model.DateFrom}',@dateTo='{model.DateTo}'").AsEnumerable().ToList();
            foreach (var orderData in OrderResponseList)
            {
                orderData.OrderItem = allOrderItems.Where(x => x.OrderId == orderData.Id).ToList();
            }
            return (OrderResponseList, totalCount);
        }


        public List<OrdersViewModel> GetOnGoingOrders()
        {
            string orderStatus = "Requested_Preparing_Prepared_Served_Delivered";
            GetRequestDTO model = new GetRequestDTO();
            model.PageNo = 0;
            model.PageSize = 1000;
            model.DateFrom = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            model.DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //var totalCount = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected" && x.OrderStatus != "Paid").CountAsync();
            var OrderResponseList = _unitOfWork.Context.OrdersViewModel.FromSqlRaw($"EXEC sp_GetAllOrders @orderStatus='{orderStatus}',@pageNo='{model.PageNo}',@pageSize='{model.PageSize}',@dateFrom='{model.DateFrom}',@dateTo='{model.DateTo}'").AsEnumerable().ToList(); ///.Where(x => x.OrderStatus != "Rejected").Include(x => x.OrderItem).ToListAsync();
            var allOrderItems = _unitOfWork.Context.OrderItemsViewModel.FromSqlRaw($"EXEC sp_GetAllOrderItems @orderStatus='{orderStatus}',@pageNo='{model.PageNo}',@pageSize='{model.PageSize}',@dateFrom='{model.DateFrom}',@dateTo='{model.DateTo}'").AsEnumerable().ToList();
            foreach (var item in OrderResponseList)
            {
                item.OrderItem = allOrderItems.Where(x => x.OrderId == item.Id).ToList();
            }
            return (OrderResponseList);
        }
        public async Task<List<Order>> GetOnGoingOrdersWOStoreProcedure()
        {
            GetRequestDTO model = new GetRequestDTO();
            model.PageNo = 0;
            model.PageSize = 1000;
            model.DateFrom = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            model.DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //var totalCount = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected" && x.OrderStatus != "Paid").CountAsync();
            var OrderResponseList = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected").Include(x => x.OrderItem).ToListAsync();

            return OrderResponseList;
        }
        public List<OrdersViewModel> GetDriveThruOrders()
        {
            string orderStatus = "Requested_Preparing_Prepared_Served";
            GetRequestDTO model = new GetRequestDTO();
            model.PageNo = 0;
            model.PageSize = 1000;
            model.DateFrom = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            model.DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var totalCount = _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected" && x.OrderStatus != "Paid").Count();
            var OrderResponseList = _unitOfWork.Context.OrdersViewModel.FromSqlRaw($"EXEC sp_GetAllOrders @orderStatus='{orderStatus}',@pageNo='{model.PageNo}',@pageSize='{model.PageSize}',@dateFrom='{model.DateFrom}',@dateTo='{model.DateTo}'").AsEnumerable().ToList().Where(x => x.OrderType == 4).ToList(); ///.Where(x => x.OrderStatus != "Rejected").Include(x => x.OrderItem).ToListAsync();
            var allOrderItems = _unitOfWork.Context.OrderItemsViewModel.FromSqlRaw($"EXEC sp_GetAllOrderItems @orderStatus='{orderStatus}',@pageNo='{model.PageNo}',@pageSize='{model.PageSize}',@dateFrom='{model.DateFrom}',@dateTo='{model.DateTo}'").AsEnumerable().ToList();
            foreach (var item in OrderResponseList)
            {
                item.OrderItem = allOrderItems.Where(x => x.OrderId == item.Id).ToList();
            }
            return OrderResponseList;
        }

        public async Task<List<OrderResponseDTO>> GetOrdersByKitchenId(int id)
        {
            var response = await _unitOfWork.Context.OrderItem.Where(x => x.KitchenId == id).ToListAsync();
            List<OrderResponseDTO> orderResponseDTO = new List<OrderResponseDTO>();
            if (response != null && response.Count > 0)
            {
                try
                {
                    bool flage = true;
                    foreach (var item in response)
                    {
                        var responseresult = await _unitOfWork.Context.Order.Where(x => x.Id == item.OrderId && (x.OrderStatus != "Rejected" && x.OrderStatus != "Prepared" && x.OrderStatus != "Served" && x.OrderStatus != "Paid") && x.OrderStatus != "Paid").Include(x => x.OrderItem).Where(c => c.OrderItem.Any(i => i.KitchenId == item.KitchenId && i.Status != "Rejected")).FirstOrDefaultAsync();
                        if (responseresult != null)
                        {
                            var OrderResponseList = _mapper.Map<OrderResponseDTO>(responseresult);
                            if (OrderResponseList != null)
                            {
                                var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).FirstOrDefaultAsync();
                                if (Table != null)
                                    OrderResponseList.TableName = Table.Name;
                                var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).FirstOrDefaultAsync();
                                if (Hall != null)
                                    OrderResponseList.HallName = Hall.Name;
                                OrderResponseList.Collapse = false;
                                var Time = OrderResponseList.CookingTime.Split(":");
                                if (Time.Length > 0)
                                    OrderResponseList.Hours = Time[0];
                                else
                                    OrderResponseList.Hours = "";
                                if (Time.Length > 1)
                                {
                                    OrderResponseList.Min = Time[1];
                                    if (OrderResponseList.Min != null)
                                    {
                                        if (OrderResponseList.StartDateTime != null)
                                        {
                                            var intmin = Convert.ToInt32(OrderResponseList.Min);
                                            var difference = (DateTime.Now - OrderResponseList.StartDateTime).ToString();
                                            var mins = difference.Split(":");
                                            OrderResponseList.RemainingTime = intmin - ((Convert.ToInt32(mins[0]) * 60) + (Convert.ToInt32(mins[1])));
                                            var sec = Convert.ToDecimal(mins[2]);
                                            OrderResponseList.RemainingSeconds = 60 - (int)sec;
                                        }
                                    }
                                }
                                else
                                    OrderResponseList.Min = "";
                                foreach (var orderItem in OrderResponseList.OrderItem)
                                {
                                    var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).FirstOrDefaultAsync();
                                    if (Kitchen != null)
                                        orderItem.KitchenName = Kitchen.Name;
                                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).FirstOrDefaultAsync();
                                    if (FoodItem != null)
                                    {
                                        orderItem.FoodItemName = FoodItem.Name;
                                        orderItem.CookingTime = FoodItem.CookingTime;
                                        orderItem.AttachmentId = FoodItem.AttachmentId;
                                        var cookTime = orderItem.CookingTime.Split(":");
                                        orderItem.Hours = cookTime[0];
                                        orderItem.Min = cookTime[1];
                                    }
                                    var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).FirstOrDefaultAsync();
                                    if (FoodVarient != null)
                                        orderItem.Name = FoodVarient.Name;
                                }
                            }
                            if (flage)
                            {
                                orderResponseDTO.Add(OrderResponseList);
                                flage = false;
                            }
                            else
                            {
                                var order = orderResponseDTO.LastOrDefault();
                                if (item.OrderId != order.Id)
                                {
                                    orderResponseDTO.Add(OrderResponseList);
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                return orderResponseDTO;
            }
            return null;
        }
        public async Task<List<OrderResponseDTO>> GetOrderByTableId(int tableId)
        {
            List<OrderResponseDTO> orderResponseDTO = new List<OrderResponseDTO>();
            try
            {
                var responseResult = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected" && x.OrderStatus != "Paid" && x.TableId == tableId).Include(x => x.OrderItem).FirstOrDefaultAsync();
                if (responseResult != null)
                {
                    var OrderResponseList = _mapper.Map<OrderResponseDTO>(responseResult);
                    foreach (var item in OrderResponseList.OrderItem)
                    {
                        if (responseResult != null)
                        {
                            if (OrderResponseList != null)
                            {
                                var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).FirstOrDefaultAsync();
                                if (Table != null)
                                    OrderResponseList.TableName = Table.Name;
                                var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).FirstOrDefaultAsync();
                                if (Hall != null)
                                    OrderResponseList.HallName = Hall.Name;
                                OrderResponseList.Collapse = false;
                                var Time = OrderResponseList.CookingTime.Split(":");
                                if (Time.Length > 0)
                                    OrderResponseList.Hours = Time[0];
                                else
                                    OrderResponseList.Hours = "";
                                if (Time.Length > 1)
                                    OrderResponseList.Min = Time[1];
                                else
                                    OrderResponseList.Min = "";
                                if (OrderResponseList.CreatedDate != null)
                                {
                                    var difference = (DateTime.Now - OrderResponseList.CreatedDate).ToString();
                                    var mins = difference.Split(":");
                                    OrderResponseList.ReserveTimeMin = Convert.ToInt32(mins[1]);
                                    var sec = Convert.ToDecimal(mins[2]);
                                    OrderResponseList.ReserveTimeHours = (int)(DateTime.Now - OrderResponseList.CreatedDate).TotalHours;
                                    OrderResponseList.ReserveTimeSeconds = (int)sec;
                                }
                                foreach (var orderItem in OrderResponseList.OrderItem)
                                {
                                    var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).FirstOrDefaultAsync();
                                    if (Kitchen != null)
                                        orderItem.KitchenName = Kitchen.Name;
                                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).FirstOrDefaultAsync();
                                    if (FoodItem != null)
                                    {
                                        orderItem.FoodItemName = FoodItem.Name;
                                        orderItem.CookingTime = FoodItem.CookingTime;
                                        orderItem.Id = orderItem.FoodVarientId;
                                        orderItem.AttachmentId = FoodItem.AttachmentId;
                                        var cookTime = orderItem.CookingTime.Split(":");
                                        orderItem.Hours = cookTime[0];
                                        orderItem.Min = cookTime[1];
                                    }
                                    var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).FirstOrDefaultAsync();
                                    if (FoodVarient != null)
                                        orderItem.Name = FoodVarient.Name;
                                }
                            }
                        }
                    }
                    orderResponseDTO.Add(OrderResponseList);
                }
                else
                    return null;
            }
            catch (Exception)
            {
                throw;
            }
            return orderResponseDTO;
        }

        public async Task<List<OrderResponseDTO>> GetDriveThruOrderswithOutStoreProcedure()
        {
            string orderStatus = "Requested_Preparing_Prepared_Served";
            GetRequestDTO model = new GetRequestDTO();
            model.PageNo = 0;
            model.PageSize = 1000;
            model.DateFrom = DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd HH:mm:ss.fff");
            model.DateTo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var totalCount = _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected" && x.OrderStatus != "Paid").Count();
            var OrderResponseData = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected" && x.OrderStatus != "Paid").Include(x => x.OrderItem).ToListAsync();
            var OrderResponse = _mapper.Map<List<OrderResponseDTO>>(OrderResponseData);
            if (OrderResponse != null)
            {
                foreach (var OrderResponseList in OrderResponse)
                {
                    var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).FirstOrDefaultAsync();
                    if (Table != null)
                        OrderResponseList.TableName = Table.Name;
                    var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).FirstOrDefaultAsync();
                    if (Hall != null)
                        OrderResponseList.HallName = Hall.Name;
                    OrderResponseList.Collapse = false;
                    var Time = OrderResponseList.CookingTime.Split(":");
                    if (Time.Length > 0)
                        OrderResponseList.Hours = Time[0];
                    else
                        OrderResponseList.Hours = "";
                    if (Time.Length > 1)
                        OrderResponseList.Min = Time[1];
                    else
                        OrderResponseList.Min = "";
                    foreach (var orderItem in OrderResponseList.OrderItem)
                    {
                        var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).FirstOrDefaultAsync();
                        if (Kitchen != null)
                            orderItem.KitchenName = Kitchen.Name;
                        var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).FirstOrDefaultAsync();
                        if (FoodItem != null)
                        {
                            orderItem.FoodItemName = FoodItem.Name;
                            orderItem.CookingTime = FoodItem.CookingTime;
                            orderItem.AttachmentId = FoodItem.AttachmentId;
                            var CookTime = orderItem.CookingTime.Split(":");
                            orderItem.Hours = Time[0];
                            orderItem.Min = Time[1];
                        }
                        var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).FirstOrDefaultAsync();
                        if (FoodVarient != null)
                            orderItem.Name = FoodVarient.Name;
                    }
                }
                return OrderResponse;
            }
            return OrderResponse;
        }
        public async Task<(List<OrderResponseDTO> oders, int totalCount)> GetAllwithOutstore(GetRequestDTO model)
        {
            //string orderStatus = "Requested_Preparing_Prepared_Served_Paid";
            var totalCount = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected").CountAsync();
            var OrderResponsedata = await _unitOfWork.Context.Order.Where(x => x.OrderStatus != "Rejected").Include(x => x.OrderItem).ToListAsync();
            var OrderResponse = _mapper.Map<List<OrderResponseDTO>>(OrderResponsedata);

            if (OrderResponse != null)
            {
                foreach (var OrderResponseList in OrderResponse)
                {
                    var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).FirstOrDefaultAsync();
                    if (Table != null)
                        OrderResponseList.TableName = Table.Name;
                    var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).FirstOrDefaultAsync();
                    if (Hall != null)
                        OrderResponseList.HallName = Hall.Name;
                    OrderResponseList.Collapse = false;
                    var Time = OrderResponseList.CookingTime.Split(":");
                    if (Time.Length > 0)
                        OrderResponseList.Hours = Time[0];
                    else
                        OrderResponseList.Hours = "";
                    if (Time.Length > 1)
                        OrderResponseList.Min = Time[1];
                    else
                        OrderResponseList.Min = "";
                    foreach (var orderItem in OrderResponseList.OrderItem)
                    {
                        var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).FirstOrDefaultAsync();
                        if (Kitchen != null)
                            orderItem.KitchenName = Kitchen.Name;
                        var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).FirstOrDefaultAsync();
                        if (FoodItem != null)
                        {
                            orderItem.FoodItemName = FoodItem.Name;
                            orderItem.CookingTime = FoodItem.CookingTime;
                            orderItem.AttachmentId = FoodItem.AttachmentId;
                            var CookTime = orderItem.CookingTime.Split(":");
                            orderItem.Hours = Time[0];
                            orderItem.Min = Time[1];
                        }
                        var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).FirstOrDefaultAsync();
                        if (FoodVarient != null)
                            orderItem.Name = FoodVarient.Name;
                    }
                }
                return (OrderResponse, totalCount);
            }
            return (OrderResponse, totalCount);
        }
        public async Task<OrderResponseDTO> GetById(int Id)
        {
            var response = await _unitOfWork.Context.Order.Where(x => x.Id == Id).Include(x => x.OrderItem).FirstOrDefaultAsync();
            var OrderResponseList = _mapper.Map<OrderResponseDTO>(response);
            if (OrderResponseList != null)
            {
                var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).FirstOrDefaultAsync();
                if (Table != null)
                    OrderResponseList.TableName = Table.Name;
                var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).FirstOrDefaultAsync();
                if (Hall != null)
                    OrderResponseList.HallName = Hall.Name;
                OrderResponseList.Collapse = false;
                var Time = OrderResponseList.CookingTime.Split(":");
                if (Time.Length > 0)
                    OrderResponseList.Hours = Time[0];
                else
                    OrderResponseList.Hours = "";
                if (Time.Length > 1)
                    OrderResponseList.Min = Time[1];
                else
                    OrderResponseList.Min = "";
                foreach (var orderItem in OrderResponseList.OrderItem)
                {
                    var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).FirstOrDefaultAsync();
                    if (Kitchen != null)
                        orderItem.KitchenName = Kitchen.Name;
                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).FirstOrDefaultAsync();
                    if (FoodItem != null)
                    {
                        orderItem.FoodItemName = FoodItem.Name;
                        orderItem.CookingTime = FoodItem.CookingTime;
                        orderItem.AttachmentId = FoodItem.AttachmentId;
                        var CookTime = orderItem.CookingTime.Split(":");
                        orderItem.Hours = Time[0];
                        orderItem.Min = Time[1];
                    }
                    var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).FirstOrDefaultAsync();
                    if (FoodVarient != null)
                        orderItem.Name = FoodVarient.Name;
                }
                return OrderResponseList;
            }
            return OrderResponseList;
        }
        public async Task<OrderResponseDTO> GetByOrderId(int Id)
        {
            var response = await _unitOfWork.Context.Order.Where(x => x.Id == Id).Include(x => x.OrderItem).SingleOrDefaultAsync();
            var OrderResponseList = _mapper.Map<OrderResponseDTO>(response);
            if (OrderResponseList != null)
            {
                var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).FirstOrDefaultAsync();
                if (Table != null)
                    OrderResponseList.TableName = Table.Name;
                var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).FirstOrDefaultAsync();
                if (Hall != null)
                    OrderResponseList.HallName = Hall.Name;
                OrderResponseList.Collapse = false;
                var Time = OrderResponseList.CookingTime.Split(":");
                if (Time.Length > 0)
                    OrderResponseList.Hours = Time[0];
                else
                    OrderResponseList.Hours = "";
                if (Time.Length > 1)
                {
                    OrderResponseList.Min = Time[1];
                    // var min = OrderResponseList.Min
                }
                else
                    OrderResponseList.Min = "";
                foreach (var orderItem in OrderResponseList.OrderItem)
                {
                    var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).FirstOrDefaultAsync();
                    if (Kitchen != null)
                        orderItem.KitchenName = Kitchen.Name;
                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).FirstOrDefaultAsync();
                    if (FoodItem != null)
                    {
                        orderItem.FoodItemName = FoodItem.Name;
                        orderItem.CookingTime = FoodItem.CookingTime;
                        orderItem.AttachmentId = FoodItem.AttachmentId;
                        var CookTime = orderItem.CookingTime.Split(":");
                        orderItem.Hours = Time[0];
                        orderItem.Min = Time[1];
                    }
                    var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).FirstOrDefaultAsync();
                    if (FoodVarient != null)
                        orderItem.Name = FoodVarient.Name;
                }
                return OrderResponseList;
            }
            return OrderResponseList;
        }
        public async Task<long> SaveOrder(OrderRequestDTO model, int Id)
        {
            Order order = new Order();
            order = _mapper.Map<Order>(model);

            if (model.Id <= 0)
            {
                order.OrderStatus = "Requested";
                order.CreatedDate = DateTime.Now;
                order.CreatedBy = Id;
                order.IsSynchronized = false;
                _unitOfWork.Context.Set<Order>().Add(order);
            }
            else
            {
                order.UpdatedBy = Id;
                var previousOrder = await _unitOfWork.Context.Order.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (previousOrder != null)
                {
                    try
                    {
                        order.UpdatedBy = Id;
                        order.IsSynchronized = false;
                        order.CreatedBy = previousOrder.CreatedBy;
                        order.CreatedDate = previousOrder.CreatedDate;
                        order.OrderStatus = previousOrder.OrderStatus;
                        order.StartDateTime = previousOrder.StartDateTime;
                        _unitOfWork.Context.Entry(order).State = EntityState.Modified;
                        _unitOfWork.Context.Order.Update(order);
                        await _unitOfWork.Commit();
                        var orderItems = await _unitOfWork.Context.OrderItem.Where(x => x.OrderId == model.Id).ToListAsync();
                        _unitOfWork.Context.OrderItem.RemoveRange(orderItems);
                        await _unitOfWork.Commit();
                        foreach (var item in model.OrderItem)
                        {
                            OrderItem orderItem = new OrderItem();
                            orderItem = _mapper.Map<OrderItem>(item);
                            orderItem.OrderId = model.Id;
                            _unitOfWork.Context.Set<OrderItem>().Add(orderItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            await _unitOfWork.Commit();
            return order.Id;
        }
        public async Task<long> SaveOrderItem(OrderItemRequestDTO model)
        {
            OrderItem orderItems = new OrderItem();
            orderItems = _mapper.Map<OrderItem>(model);
            if (model.Id <= 0)
            {
                orderItems.Status = "Requested";
                _unitOfWork.Context.Set<OrderItem>().Add(orderItems);
            }
            else
            {
                _unitOfWork.Context.Entry(orderItems).State = EntityState.Modified;
                _unitOfWork.Context.OrderItem.Update(orderItems);
            }
            await _unitOfWork.Commit();
            return orderItems.Id;
        }
        public async Task<long> AcceptedOrderStatus(int OrderId, int KitchenId, string Status)
        {
            List<OrderItem> orderList = _unitOfWork.Context.OrderItem.Where(x => x.KitchenId == KitchenId && x.OrderId == OrderId).ToList();
            foreach (var item in orderList)
            {
                item.Status = Status;
                _unitOfWork.Context.Entry(item).State = EntityState.Modified;
                _unitOfWork.Context.OrderItem.Update(item);
                await _unitOfWork.Commit();
            }
            return 1;
        }
        public async Task<long> PreparedOrderStatus(int OrderId, int KitchenId, string Status)
        {
            List<OrderItem> orderList = _unitOfWork.Context.OrderItem.Where(x => x.KitchenId == KitchenId && x.OrderId == OrderId).ToList();
            foreach (var item in orderList)
            {
                item.Status = Status;
                _unitOfWork.Context.Entry(item).State = EntityState.Modified;
                _unitOfWork.Context.OrderItem.Update(item);
                await _unitOfWork.Commit();
            }
            return 1;
        }
        public async Task<long> UpdateOrderStatus(int OrderId, string Status)
        {
            Order order = _unitOfWork.Context.Order.Where(x => x.Id == OrderId).FirstOrDefault();
            if (order != null)
            {
                order.OrderStatus = Status;
                if (Status == "Preparing")
                    order.StartDateTime = DateTime.Now;
                else if (Status == "Prepared")
                    order.PreparedTime = DateTime.Now;
                else if (Status == "Served")
                    order.ServedTime = DateTime.Now;
                else if (Status == "Paid")
                {
                    order.PaidStatus = true;
                    order.CompleteTime = DateTime.Now;
                    //var data = await _tableRepository.SaveStatus(model.TableId, 1);
                }
                else
                    order.PaidStatus = false;
                _unitOfWork.Context.Entry(order).State = EntityState.Modified;
                _unitOfWork.Context.Order.Update(order);
                await _unitOfWork.Commit();
            }
            return 1;
        }
        public async Task<long> UpdateOrderType(UpdateOrderTypeRequestDTO model)
        {
            Order order = _unitOfWork.Context.Order.Where(x => x.Id == model.OrderId).FirstOrDefault();
            if (order != null)
            {
                if (model.HallId == 0 && model.TableId == 0)
                {
                    order.HallId = null;
                    order.TableId = null;
                    order.OrderType = model.OrderType;
                }
                else
                {
                    order.HallId = model.HallId;
                    order.TableId = model.TableId;
                    order.OrderType = model.OrderType;
                }
                _unitOfWork.Context.Entry(order).State = EntityState.Modified;
                _unitOfWork.Context.Order.Update(order);
                await _unitOfWork.Commit();
                return 1;
            }
            else
            {
                return 0;
            }
        }
        public async Task<long> UpdateItemStatus(int OrderId, int FoodItemId, int FoodVarientId, string Status)
        {
            OrderItem orderItem = _unitOfWork.Context.OrderItem.Where(x => x.FoodItemId == FoodItemId && x.OrderId == OrderId && x.FoodVarientId == FoodVarientId && x.Status != "Rejected").FirstOrDefault();
            if (orderItem != null)
            {
                orderItem.Status = Status;
                _unitOfWork.Context.Entry(orderItem).State = EntityState.Modified;
                _unitOfWork.Context.OrderItem.Update(orderItem);
                await _unitOfWork.Commit();
            }
            return 1;
        }
        public async Task<(IEnumerable<OrderResponseDTO> OrderList, int recordsCount)> GetByFilters(SearchFilter filters)
        {
            List<Order> listData = new List<Order>();

            var predicate = PredicateBuilder.New<Order>(true);
            bool isPradicate = false;
            //predicate = predicate.And(i => i.OrderStatus == false);
            //if (!string.IsNullOrEmpty(filters.SearchText))
            //{
            //    predicate = predicate.Or(i => i.CreatedDate(filters.SearchText));
            //    predicate = predicate.Or(i => i.HallId(filters.SearchText));
            //    predicate = predicate.Or(i => i.TableId(filters.SearchText));
            //    predicate = predicate.Or(i => i.OrderStatus(filters.SearchText));
            //    isPradicate = true;
            //}
            if (filters.Filters != null)
                foreach (var filter in filters.Filters)
                {
                    if (filter.Logic == "and")
                        predicate.And(DBHelper.BuildPredicate<Order>(filter.Name, filter.Operator, filter.Value));
                    else if (filter.Logic == "or")
                        predicate.Or(DBHelper.BuildPredicate<Order>(filter.Name, filter.Operator, filter.Value));
                    else
                        predicate.And(DBHelper.BuildPredicate<Order>(filter.Name, filter.Operator, filter.Value));
                    isPradicate = true;
                }
            if (isPradicate)
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.Order.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            else
            {
                var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
                listData = await _unitOfWork.Context.Order.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
            }
            var recordsCount = await _unitOfWork.Context.Order.Where(x => x.OrderStatus == "Pending").CountAsync();
            (List<OrderResponseDTO> OrderList, int recordsCount) tuple = (_mapper.Map<List<OrderResponseDTO>>(listData), recordsCount);
            return tuple;
        }
        public async Task<List<OrderItem>> OrderItemByOrderId(int orderId)
        {
            List<OrderItem> orderList = await _unitOfWork.Context.OrderItem.Where(x => x.OrderId == orderId && x.Status == "Requested" && x.Status != "Rejected").ToListAsync();
            if (orderList != null && orderList.Count > 0)
                return orderList;
            else
                return null;
        }

        public async Task<IEnumerable<OrderResponseDTO>> SearchByInvoiceId(int Id)
        {
            var response = await _unitOfWork.Context.Order.FromSqlRaw($"Select * from dbo.[Order] Where Id like '{Id}%' AND OrderStatus != 'Paid' AND OrderStatus != 'Rejected'").Include(x => x.OrderItem).ToListAsync();

            var OrderResponseList = _mapper.Map<List<OrderResponseDTO>>(response);
            foreach (var item in OrderResponseList)
            {
                var Table = await _unitOfWork.Context.Table.Where(x => x.Id == item.TableId).SingleOrDefaultAsync();
                if (Table != null)
                    item.TableName = Table.Name;
                var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == item.HallId).SingleOrDefaultAsync();
                if (Hall != null)
                    item.HallName = Hall.Name;
                item.Collapse = false;
                var Time = item.CookingTime.Split(":");
                item.Hours = Time[0];
                item.Min = Time[1];
                foreach (var orderItem in item.OrderItem)
                {
                    var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).SingleOrDefaultAsync();
                    if (Kitchen != null)
                        orderItem.KitchenName = Kitchen.Name;
                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).SingleOrDefaultAsync();
                    if (FoodItem != null)
                    {
                        orderItem.FoodItemName = FoodItem.Name;
                        orderItem.CookingTime = FoodItem.CookingTime;
                        orderItem.AttachmentId = FoodItem.AttachmentId;
                        var CookTime = orderItem.CookingTime.Split(":");
                        orderItem.Hours = Time[0];
                        orderItem.Min = Time[1];
                    }
                    var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).SingleOrDefaultAsync();
                    if (FoodVarient != null)
                        orderItem.Name = FoodVarient.Name;
                }
            }
            return OrderResponseList;
        }
    }
    public interface IOrderRepository
    {
        Task<(IEnumerable<OrdersViewModel>, int totalCount)> GetAll(GetRequestDTO model);
        //List<OrderResponseDTO> GetOnGoingOrders();
        Task<(List<OrderResponseDTO> oders, int totalCount)> GetAllwithOutstore(GetRequestDTO model);
        List<OrdersViewModel> GetOnGoingOrders();
        Task<List<Order>> GetOnGoingOrdersWOStoreProcedure();
        List<OrdersViewModel> GetDriveThruOrders();
        Task<List<OrderResponseDTO>> GetDriveThruOrderswithOutStoreProcedure();
        Task<IEnumerable<OrderResponseDTO>> SearchByInvoiceId(int Id);
        Task<OrderResponseDTO> GetById(int Id);
        Task<OrderResponseDTO> GetByOrderId(int Id);
        Task<List<OrderResponseDTO>> GetOrdersByKitchenId(int id);
        Task<List<OrderResponseDTO>> GetOrderByTableId(int TableId);
        Task<long> SaveOrder(OrderRequestDTO model, int Id);
        Task<long> SaveOrderItem(OrderItemRequestDTO model);
        Task<List<OrderItem>> OrderItemByOrderId(int orderId);
        Task<long> AcceptedOrderStatus(int OrderId, int KitchenId, string Status);
        Task<long> PreparedOrderStatus(int OrderId, int KitchenId, string Status);
        Task<long> UpdateOrderStatus(int Id, string Status);
        Task<long> UpdateOrderType(UpdateOrderTypeRequestDTO model);
        Task<long> UpdateItemStatus(int OrderId, int FoodItemId, int FoodVarientId, string Status);
        Task<(IEnumerable<OrderResponseDTO> OrderList, int recordsCount)> GetByFilters(SearchFilter filters);
        Task<List<Order>> GetALLSync();
    }
}