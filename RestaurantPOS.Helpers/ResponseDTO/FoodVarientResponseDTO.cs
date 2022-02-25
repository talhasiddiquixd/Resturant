using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public  class FoodVarientResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public int FoodItemId { get; set; }
        public string FoodItemName { get; set; }
        public string CookingTime { get; set; }
        public int? KitchenId { get; set; }
    }
}
