using System;
using AutoMapper;
using System.Linq;
using RestaurantPOS.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using RestaurantPOS.Helpers.RequestDTO;
using RestaurantPOS.Helpers.ResponseDTO;
namespace RestaurantPOS.Repository
{
    public class ReportsRepository : IReportsRepository
    {
        private readonly IunitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReportsRepository(IunitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<OrderResponseDTO>> GetAllOrders(GetAllOrdersRequestDTO model)
        {
            var response = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date).Include(x => x.OrderItem).ToListAsync();
            var OrderResponseList = _mapper.Map<List<OrderResponseDTO>>(response);
            foreach (var item in OrderResponseList)
            {
                var TableName = await _unitOfWork.Context.Table.Where(x => x.Id == item.TableId).Select(x => x.Name).SingleOrDefaultAsync();
                if (!string.IsNullOrEmpty(TableName))
                    item.TableName = TableName;
                var HallName = await _unitOfWork.Context.Hall.Where(x => x.Id == item.HallId).Select(x => x.Name).SingleOrDefaultAsync();
                if (!String.IsNullOrEmpty(HallName))
                    item.HallName = HallName;
                var Time = item.CookingTime.Split(":");
                item.Hours = Time[0];
                item.Min = Time[1];
                foreach (var orderItem in item.OrderItem)
                {
                    var KitchenName = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).Select(x => x.Name).SingleOrDefaultAsync();
                    if (!string.IsNullOrEmpty(KitchenName))
                        orderItem.KitchenName = KitchenName;
                    var FoodItemName = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).Select(x => x.Name).SingleOrDefaultAsync();
                    if (!string.IsNullOrEmpty(FoodItemName))
                        orderItem.FoodItemName = FoodItemName;
                    var FoodVarientName = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).Select(x => x.Name).SingleOrDefaultAsync();
                    if (!string.IsNullOrEmpty(FoodVarientName))
                        orderItem.Name = FoodVarientName;
                }
            }
            return OrderResponseList;
        }
        public async Task<List<OrderResponseDTO>> GetAllOrdersForDashboard(GetAllOrdersRequestDTO model)
        {
            var response = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date).ToListAsync();
            var OrderResponseList = _mapper.Map<List<OrderResponseDTO>>(response);
            return OrderResponseList;
        }
        public async Task<OrderResponseDTO> GetByOrderId(int OrderId)
        {
            var response = await _unitOfWork.Context.Order.Where(x => x.Id == OrderId).Include(x => x.OrderItem).FirstOrDefaultAsync();
            var OrderResponseList = _mapper.Map<OrderResponseDTO>(response);
            var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).SingleOrDefaultAsync();
            if (Table != null)
                OrderResponseList.TableName = Table.Name;
            var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).SingleOrDefaultAsync();
            if (Hall != null)
                OrderResponseList.HallName = Hall.Name;
            var Time = OrderResponseList.CookingTime.Split(":");
            OrderResponseList.Hours = Time[0];
            OrderResponseList.Min = Time[1];
            foreach (var orderItem in OrderResponseList.OrderItem)
            {
                var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).SingleOrDefaultAsync();
                if (Kitchen != null)
                    orderItem.KitchenName = Kitchen.Name;
                var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).SingleOrDefaultAsync();
                if (FoodItem != null)
                    orderItem.FoodItemName = FoodItem.Name;
                var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).SingleOrDefaultAsync();
                if (FoodVarient != null)
                    orderItem.Name = FoodVarient.Name;
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
                _unitOfWork.Context.Entry(order).State = EntityState.Modified;
                _unitOfWork.Context.Order.Update(order);
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
        public async Task<long> UpdateOrderStatus(int Id, string Status)
        {
            Order order = _unitOfWork.Context.Order.Where(x => x.Id.Equals(Id)).FirstOrDefault();
            if (order != null)
            {
                order.OrderStatus = Status;
                _unitOfWork.Context.Entry(order).State = EntityState.Modified;
                _unitOfWork.Context.Order.Update(order);
                await _unitOfWork.Commit();
            }
            return 1;
        }
        public async Task<long> UpdateItemStatus(int Id, string Status)
        {
            OrderItem orderItem = _unitOfWork.Context.OrderItem.Where(x => x.Id.Equals(Id)).FirstOrDefault();
            if (orderItem != null)
            {
                orderItem.Status = Status;
                _unitOfWork.Context.Entry(orderItem).State = EntityState.Modified;
                _unitOfWork.Context.OrderItem.Update(orderItem);
                await _unitOfWork.Commit();
            }
            return 1;
        }
        //public async Task<(IEnumerable<OrderResponseDTO> OrderList, int recordsCount)> GetByFilters(SearchFilter filters)
        //{
        //    List<Order> listData = new List<Order>();
        //    var predicate = PredicateBuilder.New<Order>(true);
        //    bool isPradicate = false;
        //    predicate = predicate.And(i => i.OrderStatus == "");
        //    if (!string.IsNullOrEmpty(filters.SearchText))
        //    {
        //        predicate = predicate.Or(i => i.CreatedDate(filters.SearchText));
        //        isPradicate = true;
        //    }
        //    if (filters.Filters != null)
        //        foreach (var filter in filters.Filters)
        //        {
        //            if (filter.Logic == "and")
        //                predicate.And(DBHelper.BuildPredicate<Order>(filter.Name, filter.Operator, filter.Value));
        //            else if (filter.Logic == "or")
        //                predicate.Or(DBHelper.BuildPredicate<Order>(filter.Name, filter.Operator, filter.Value));
        //            else
        //                predicate.And(DBHelper.BuildPredicate<Order>(filter.Name, filter.Operator, filter.Value));
        //            isPradicate = true;
        //        }
        //    if (isPradicate)
        //    {
        //        var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
        //        listData = await _unitOfWork.Context.Order.Where(predicate).Skip(filters.Offset * filters.PageSize).Take(filters.PageSize).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
        //    }
        //    else
        //    {
        //        var direction = (filters.sortFilter?.Direction?.ToLower().Equals("asc") ?? false) ? DBHelper.Order.Asc : DBHelper.Order.Desc;
        //        listData = await _unitOfWork.Context.Order.Skip(filters.Offset * filters.PageSize).Take(filters.Offset).OrderByDynamic(filters.sortFilter.SortBy, direction).ToListAsync();
        //    }
        //    var recordsCount = await _unitOfWork.Context.Order.Where(x => x.OrderStatus == "Pending").CountAsync();
        //    (List<OrderResponseDTO> OrderList, int recordsCount) tuple = (_mapper.Map<List<OrderResponseDTO>>(listData), recordsCount);
        //    return tuple;
        //}
        public async Task<List<OrderResponseDTO>> GetOrdersByKitchenId(KitshensOrderRequestDTO model)
        {
            var response = await _unitOfWork.Context.OrderItem.Where(x => x.KitchenId == model.KitchenId).ToListAsync();
            List<OrderResponseDTO> orderResponseDTO = new List<OrderResponseDTO>();
            foreach (var item in response)
            {
                var responseResult = await _unitOfWork.Context.Order.Where(x => x.Id == item.OrderId && x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date).Include(x => x.OrderItem).SingleOrDefaultAsync();
                if (responseResult != null)
                {
                    var OrderResponseList = _mapper.Map<OrderResponseDTO>(responseResult);
                    var Table = await _unitOfWork.Context.Table.Where(x => x.Id == OrderResponseList.TableId).SingleOrDefaultAsync();
                    if (Table != null)
                        OrderResponseList.TableName = Table.Name;
                    var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == OrderResponseList.HallId).SingleOrDefaultAsync();
                    if (Hall != null)
                        OrderResponseList.HallName = Hall.Name;
                    var Time = responseResult.CookingTime.Split(":");
                    OrderResponseList.Hours = Time[0];
                    OrderResponseList.Min = Time[1];
                    foreach (var orderItem in OrderResponseList.OrderItem)
                    {
                        var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).SingleOrDefaultAsync();
                        if (Kitchen != null)
                            orderItem.KitchenName = Kitchen.Name;
                        var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).SingleOrDefaultAsync();
                        if (FoodItem != null)
                            orderItem.FoodItemName = FoodItem.Name;
                        var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).SingleOrDefaultAsync();
                        if (FoodVarient != null)
                            orderItem.Name = FoodVarient.Name;
                        var CookTime = orderItem.CookingTime.Split(":");
                        orderItem.Hours = CookTime[0];
                        orderItem.Min = CookTime[1];
                    }
                    orderResponseDTO.Add(OrderResponseList);
                }
            }
            return orderResponseDTO;
        }
        public async Task<List<HallReportResponseDTO>> HallReport(GetAllOrdersRequestDTO model)
        {
            var HallList = await _unitOfWork.Context.Hall.ToListAsync();
            List<HallReportResponseDTO> orderResponseDTO = new List<HallReportResponseDTO>();
            foreach (var hall in HallList)
            {
                var response = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date && x.HallId == hall.Id).ToListAsync();
                decimal Sum = 0;
                var HallName = new Hall();
                foreach (var item in response)
                {
                    Sum += (decimal)item.TotalAmount;
                }
                HallName = await _unitOfWork.Context.Hall.Where(x => x.Id >= hall.Id).FirstOrDefaultAsync();
                orderResponseDTO.Add(new HallReportResponseDTO()
                {
                    Total = Sum,
                    HallName = HallName.Name,
                });
            }
            return orderResponseDTO;
        }
        public async Task<List<OrderReportResponseDTO>> OrderTypesReport(GetAllOrdersRequestDTO model)
        {
            var OrderList = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date).ToListAsync();
            List<OrderReportResponseDTO> orderResponseDTO = new List<OrderReportResponseDTO>();
            foreach (var orderData in OrderList.GroupBy(x => x.OrderType))
            {
                var ordertype = orderData.Key;
                decimal Sum = orderData?.ToList()?.Sum(x => x.TotalAmount) ?? 0;
                var HallName = new Hall();
                orderResponseDTO.Add(new OrderReportResponseDTO()
                {
                    Total = Sum,
                    OrderType = ordertype
                });
            }
            return orderResponseDTO;
        }
        public async Task<List<OrderResponseDTO>> SpecificOrderTypesReport(GetSpecificOrdersRequestDTO model)
        {
            var response = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date && x.OrderType == model.OrderType).Include(x => x.OrderItem).ToListAsync();
            var OrderResponseList = _mapper.Map<List<OrderResponseDTO>>(response);
            foreach (var item in OrderResponseList)
            {
                var Table = await _unitOfWork.Context.Table.Where(x => x.Id == item.TableId).SingleOrDefaultAsync();
                if (Table != null)
                    item.TableName = Table.Name;
                var Hall = await _unitOfWork.Context.Hall.Where(x => x.Id == item.HallId).SingleOrDefaultAsync();
                if (Hall != null)
                    item.HallName = Hall.Name;
                foreach (var orderItem in item.OrderItem)
                {
                    var Kitchen = await _unitOfWork.Context.Kitchen.Where(x => x.Id == orderItem.KitchenId).SingleOrDefaultAsync();
                    if (Kitchen != null)
                        orderItem.KitchenName = Kitchen.Name;
                    var FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == orderItem.FoodItemId).SingleOrDefaultAsync();
                    if (FoodItem != null)
                        orderItem.FoodItemName = FoodItem.Name;
                    var FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == orderItem.FoodVarientId).SingleOrDefaultAsync();
                    if (FoodVarient != null)
                        orderItem.Name = FoodVarient.Name;
                }
            }
            return OrderResponseList;
            //var OrderList = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date && x.OrderType == model.OrderType).ToListAsync();
            //decimal? Sum = 0;
            //OrderReportResponseDTO orderResponseDTO = new OrderReportResponseDTO();
            //foreach (var orderData in OrderList)
            //{
            //    //var response = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.To.Date && x.CreatedDate.Date <= model.From.Date && x.HallId == Order.Id).ToListAsync();
            //    Sum += orderData.TotalAmount;
            //}
            //orderResponseDTO = new OrderReportResponseDTO()
            //{
            //    Total = Sum,
            //    OrderType = model.OrderType
            //};
            //return orderResponseDTO;
        }
        public async Task<OrderChargesReportResponseDTO> ChargesOnOrdersReport(GetAllOrdersRequestDTO model)
        {
            var response = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date).ToListAsync();
            var OrderResponseList = new OrderChargesReportResponseDTO();
            OrderResponseList.TotalDeliveryCharges = 0;
            OrderResponseList.TotalServiceCharges = 0;
            OrderResponseList.TotalTax = 0;
            foreach (var item in response)
            {
                OrderResponseList.TotalDeliveryCharges += item.DeliveryCharges;
                OrderResponseList.TotalServiceCharges += item.ServiceCharges;
                OrderResponseList.TotalTax += item.Tax;
            }
            return OrderResponseList;
        }
        public async Task<List<KitchenReportResponseDTO>> KitchenReport(GetAllOrdersRequestDTO model)
        {
            var KitchenList = await _unitOfWork.Context.Kitchen.ToListAsync();
            List<KitchenReportResponseDTO> orderResponseDTO = new List<KitchenReportResponseDTO>();
            var response = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date).ToListAsync();
            string ResponsekitchenName = "";
            foreach (var kitchen in KitchenList)
            {
                decimal Sum = 0;
                var KitchenName = new Kitchen();
                foreach (var item in response)
                {
                    var KitchenTotal = await _unitOfWork.Context.OrderItem.Where(x => x.OrderId == item.Id && x.KitchenId == kitchen.Id).ToListAsync();
                    foreach (var total in KitchenTotal)
                    {
                        Sum += (decimal)total.Total;
                    }
                }
                KitchenName = await _unitOfWork.Context.Kitchen.Where(x => x.Id == kitchen.Id).FirstOrDefaultAsync();
                if (KitchenName != null)
                    ResponsekitchenName = KitchenName.Name;
                orderResponseDTO.Add(new KitchenReportResponseDTO()
                {
                    Total = Sum,
                    KitchenName = ResponsekitchenName,
                });
            }
            return orderResponseDTO;
        }
        public async Task<List<OrderItemReportResponseDTO>> OrderitemReport(GetAllOrdersRequestDTO model)
        {
            var FoodItemList = await _unitOfWork.Context.FoodItem.Include(x => x.FoodVarient).ToListAsync();
            var OrderList = await _unitOfWork.Context.Order.Where(x => x.CreatedDate.Date >= model.From.Date && x.CreatedDate.Date <= model.To.Date).ToListAsync();
            List<OrderItemReportResponseDTO> orderResponseDTO = new List<OrderItemReportResponseDTO>();
            string FoodItemName = "";
            string VariantName = "";
            foreach (var item in FoodItemList)
            {
                decimal Sum = 0;
                var FoodItem = new FoodItem();
                var FoodVarient = new FoodVarient();
                decimal? salecount = 0;
                foreach (var varient in item.FoodVarient)
                {
                    foreach (var orderData in OrderList)
                    {
                        var ItemTotal = await _unitOfWork.Context.OrderItem.Where(x => x.OrderId == orderData.Id && x.FoodItemId == item.Id && x.FoodVarientId == varient.Id).FirstOrDefaultAsync();
                        if (ItemTotal != null)
                        {
                            Sum += (decimal)ItemTotal.Total;
                            salecount += ItemTotal.Quantity;
                        }
                    }
                    FoodItem = await _unitOfWork.Context.FoodItem.Where(x => x.Id == item.Id).FirstOrDefaultAsync();
                    if (FoodItem != null)
                        FoodItemName = FoodItem.Name;
                    FoodVarient = await _unitOfWork.Context.FoodVarient.Where(x => x.Id == varient.Id).FirstOrDefaultAsync();
                    if (FoodVarient != null)
                        VariantName = FoodVarient.Name;
                    orderResponseDTO.Add(new OrderItemReportResponseDTO()
                    {
                        ItemName = FoodItemName,
                        SaleQuantity = salecount,
                        VariantName = VariantName,
                        Total = Sum
                    });
                }
            }
            return orderResponseDTO;
        }
    }
    public interface IReportsRepository
    {
        Task<List<OrderResponseDTO>> GetAllOrders(GetAllOrdersRequestDTO model);
        Task<List<OrderResponseDTO>> GetAllOrdersForDashboard(GetAllOrdersRequestDTO model);
        Task<OrderResponseDTO> GetByOrderId(int OrderId);
        Task<List<OrderResponseDTO>> GetOrdersByKitchenId(KitshensOrderRequestDTO model);
        Task<List<KitchenReportResponseDTO>> KitchenReport(GetAllOrdersRequestDTO model);
        Task<List<HallReportResponseDTO>> HallReport(GetAllOrdersRequestDTO model);
        Task<List<OrderReportResponseDTO>> OrderTypesReport(GetAllOrdersRequestDTO model);
        Task<List<OrderResponseDTO>> SpecificOrderTypesReport(GetSpecificOrdersRequestDTO model);
        Task<OrderChargesReportResponseDTO> ChargesOnOrdersReport(GetAllOrdersRequestDTO model);
        Task<List<OrderItemReportResponseDTO>> OrderitemReport(GetAllOrdersRequestDTO model);
        Task<long> SaveOrder(OrderRequestDTO model, int Id);
        Task<long> SaveOrderItem(OrderItemRequestDTO model);
        Task<long> AcceptedOrderStatus(int OrderId, int KitchenId, string Status);
        Task<long> PreparedOrderStatus(int OrderId, int KitchenId, string Status);
        Task<long> UpdateOrderStatus(int Id, string Status);
        Task<long> UpdateItemStatus(int Id, string Status);
        //Task<(IEnumerable<OrderResponseDTO> OrderList, int recordsCount)> GetByFilters(SearchFilter filters);
    }
}