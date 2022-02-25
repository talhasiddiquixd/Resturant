using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class OrderResponseDTO
    {
        public int Id { get; set; }
        public int HallId { get; set; }
        public string HallName { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public decimal Amount { get; set; }
        public decimal? Discount { get; set; }
        public string CookingTime { get; set; }
        public string Hours { get; set; }
        public string Min { get; set; }
        public decimal? ServiceCharges { get; set; }
        public decimal? DeliveryCharges { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Tax { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReserveTimeHours { get; set; }
        public int ReserveTimeMin { get; set; }
        public int ReserveTimeSeconds { get; set; }
        public DateTime? StartDateTime { get; set; }
        public int RemainingTime { get; set; }
        public int RemainingSeconds { get; set; }
        public int OrdersCount { get; set; }
        public string OrderStatus { get; set; }
        public int OrderType { get; set; }
        public bool Collapse { get; set; }
        public DateTime? PreparedTime { get; set; }
        public DateTime? ServedTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public List<OrderItemResponseDTO> OrderItem { get; set; }
        public List<TableResponseDTO> Tables { get; set; }
    }
    public class OrderChargesReportResponseDTO
    {
       
        public decimal? TotalServiceCharges { get; set; }
        public decimal? TotalDeliveryCharges { get; set; }
        public decimal? TotalTax { get; set; }
        
    }
    public class OrderItemResponseDTO
    {
        public int OrderId { get; set; }
        public int KitchenId { get; set; }
        public string KitchenName { get; set; }
        public int FoodItemId { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public string FoodItemName { get; set; }
        public string CookingTime { get; set; }
        public DateTime? PreparedTime { get; set; }
        public DateTime? ServedTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public string Hours { get; set; }
        public string Min { get; set; }
        public int FoodVarientId { get; set; }
        // field Id is only for updating order 
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}