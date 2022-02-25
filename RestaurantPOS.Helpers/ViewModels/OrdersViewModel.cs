using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Helpers.ViewModels
{
    public class OrdersViewModel
    {
        public int Id { get; set; }
        public int? HallId { get; set; }
        public string HallName { get; set; }
        public int? TableId { get; set; }
        public string TableName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }
        public string CookingTime { get; set; }
        public string Hours { get; set; }
        public string Min { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ServiceCharges { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DeliveryCharges { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Tax { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ReserveTimeHours { get; set; }
        public int? ReserveTimeMin { get; set; }
        public int? ReserveTimeSeconds { get; set; }
        public DateTime? StartDateTime { get; set; }
        public int? RemainingTime { get; set; }
        public int? RemainingSeconds { get; set; }
        public string OrderStatus { get; set; }
        public int? OrderType { get; set; }
        public DateTime? PreparedTime { get; set; }
        public DateTime? ServedTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public List<OrderItemsViewModel> OrderItem { get; set; }
        //public List<TablesViewModel> Tables { get; set; }
    }
}
