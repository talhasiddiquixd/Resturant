using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Helpers.ViewModels
{
    public class OrderItemsViewModel
    {
        public int OrderId { get; set; }
        public int KitchenId { get; set; }
        public string KitchenName { get; set; }
        public int FoodItemId { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public string FoodItemName { get; set; }
        public string CookingTime { get; set; }
        public string Hours { get; set; }
        public string Min { get; set; }
        public int FoodVarientId { get; set; }
        // field Id is only for updating order 
        public int Id { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }
        public string Status { get; set; }
    }
}
