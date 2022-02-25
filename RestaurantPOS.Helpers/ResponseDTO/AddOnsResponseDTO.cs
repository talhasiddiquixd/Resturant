using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class AddOnsResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
