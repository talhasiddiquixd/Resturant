using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
public  class KitchenRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public int? CreatedBy { get; set; }
       // public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
       // public DateTime? UpdatedAt { get; set; }

    }
}
