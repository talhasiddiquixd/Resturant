using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class FoodItemRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string VAT { get; set; }
        public int FoodCategoryId { get; set; }
        public int KitchenId { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public string CookingTime { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsOffer { get; set; }
        public bool IsSpecial { get; set; }
        public bool Status { get; set; }

        // FoodItemOfferRequestDTO
        public int FoodItemId { get; set; }
        public string OfferName { get; set; }
        public decimal OfferPrice { get; set; }
        public DateTime? OfferStart { get; set; }
        public DateTime? OfferEnd { get; set; }
        public bool IsActive { get; set; }
    }
    public class FoodItemOfferRequestDTO
    {
        public int Id { get; set; }
        public int FoodItemId { get; set; }
        public string OfferName { get; set; }
        public decimal OfferPrice { get; set; }
        public DateTime? OfferStart { get; set; }
        public DateTime? OfferEnd { get; set; }
        public bool IsActive { get; set; }
    }
    public class SearchFoodItemRequestDTO
    {
        public int? Id { get; set; }
        public string Name { get; set; }
    }
}