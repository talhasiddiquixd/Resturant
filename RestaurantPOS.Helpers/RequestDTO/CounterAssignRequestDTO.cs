using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class CounterAssignRequestDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CounterId { get; set; }
  
    }
}
