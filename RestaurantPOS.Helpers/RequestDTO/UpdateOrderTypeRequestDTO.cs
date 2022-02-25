using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class UpdateOrderTypeRequestDTO
    {
        public int OrderId { get; set; }
        public int OrderType { get; set; }
        public int? HallId { get; set; }
        public int? TableId { get; set; }
    }
}