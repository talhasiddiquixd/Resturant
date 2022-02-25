using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class OrderRequestDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int? HallId { get; set; }
        public int? TableId { get; set; }
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
    public class OrderItemRequestDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int KitchenId { get; set; }
        public int FoodItemId { get; set; }
        public int FoodVarientId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}