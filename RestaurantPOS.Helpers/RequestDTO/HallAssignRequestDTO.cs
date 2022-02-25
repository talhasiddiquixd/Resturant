using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class HallAssignRequestDTO
    { 
        public int Id { get; set;}
        public int UserId { get; set;}
        public int HallId { get; set;}
    }
}
