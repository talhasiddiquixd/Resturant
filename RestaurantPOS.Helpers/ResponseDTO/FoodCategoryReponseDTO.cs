using System;
using System.Collections.Generic;
namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class FoodCategoryReponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsOffer { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<FoodCategoryOfferResponseDTO> FoodCategoryOffer { get; set; }
        public List<FoodItemResponseDTO> FoodItem { get; set; }
        //public List<SearchFoodItemResponseDTO> searchresponse { get; set; }
        public List<FoodVarientResponseDTO> FoodVarient { get; set; }
    }
    public class FoodCategoryOfferResponseDTO
    {
        public int Id { get; set; }
        public int FoodCategoryId { get; set; }
        public string OfferName { get; set; }
        public DateTime? OfferStart { get; set; }
        public DateTime? OfferEnd { get; set; }
        public bool? IsActive { get; set; }
    }
}