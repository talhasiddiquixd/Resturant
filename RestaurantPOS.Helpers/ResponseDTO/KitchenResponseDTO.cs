using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
public  class KitchenResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Collapse { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
