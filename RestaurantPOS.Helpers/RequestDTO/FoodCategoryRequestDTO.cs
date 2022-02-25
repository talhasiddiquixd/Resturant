using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class FoodCategoryRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        //public string Description { get; set; }
        public int? AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public bool? IsOffer { get; set; }
        public string OfferName { get; set; }
        public DateTime? OfferStart { get; set; }
        public DateTime? OfferEnd { get; set; }
        public bool? IsActive { get; set; }
    }

    public class FoodCategoryOfferRequestDTO
    {
        public int Id { get; set; }
        public int FoodCategoryId { get; set; }
        public string OfferName { get; set; }
        public DateTime? OfferStart { get; set; }
        public DateTime? OfferEnd { get; set; }
        public bool? IsActive { get; set; }
    }
}