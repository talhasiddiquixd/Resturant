using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class KitchenAssignRequestDTO
    {
        public int Id { get; set; }
        public int KitchenId { get; set; }
        public int UserId { get; set; }
    }
}
