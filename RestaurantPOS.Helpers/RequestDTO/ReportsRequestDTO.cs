using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class ReportsRequestDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int HallId { get; set; }
        public int TableId { get; set; }
        public string CookingTime { get; set; }
        public bool PaidStatus { get; set; }
        public decimal? Discount { get; set; }
        public decimal? ServiceCharges { get; set; }
        public decimal? DeliveryCharges { get; set; }
        public decimal? Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public int? OrderType { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<OrderItemRequestDTO> OrderItem { get; set; }
    }

    public class GetAllOrdersRequestDTO
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
    public class GetSpecificOrdersRequestDTO
    {
        public int OrderType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }

    public class KitchenReportResponseDTO
    {
        public string KitchenName { get; set; }
        public decimal Total { get; set; }
    }

    public class HallReportResponseDTO
    {
        public string HallName { get; set; }
        public decimal Total { get; set; }
    }
    public class OrderReportResponseDTO
    {
        public int? OrderType { get; set; }
        public decimal? Total { get; set; }
    }
    public class OrderItemReportResponseDTO
    {
        public string ItemName { get; set; }
        public string VariantName { get; set; }
        public decimal? SaleQuantity { get; set; }
        public decimal? Total { get; set; }
    }

}