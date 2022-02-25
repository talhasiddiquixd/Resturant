using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class ResturantChargesResponseDTO
    {
        public int Id { get; set; }
        public decimal? ServiceCharges { get; set; }
        public decimal? DeliveryCharges { get; set; }
        public decimal? Tax { get; set; }
    }
}
