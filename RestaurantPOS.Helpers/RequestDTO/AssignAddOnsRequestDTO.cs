using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class AssignAddOnsRequestDTO
    {
        public int Id { get; set; }
        public int AddOnsId { get; set; }
        public int FoodVarientId { get; set; }
    }
}
