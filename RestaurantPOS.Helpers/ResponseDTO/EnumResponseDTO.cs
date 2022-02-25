using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class EnumResponseDTO
    {
        public Dictionary<string, int> OrderTypes { get; set; }
        public Dictionary<string, int> OrderStatus { get; set; }
    }
}
