
using System;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class KitshensOrderRequestDTO
    {
        public int KitchenId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
