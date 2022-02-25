using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class HallAssignResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int HallId { get; set; }
        public string  UserName { get; set; }
        public string HallName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
