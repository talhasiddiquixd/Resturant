using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class AssignAddOnsResponseDTO
    {
        public int Id { get; set; }
        public int AddOnsId { get; set; }
        public string AddOnsName { get; set; }
        public decimal Price { get; set; }
        public int FoodVarientId { get; set; }
        public string FoodVarientName{ get; set; }
        public bool? IsSynchronized { get; set; }
    }
}
