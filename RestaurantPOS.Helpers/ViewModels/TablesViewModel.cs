using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ViewModels
{
    public class TablesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int HallId { get; set; }
        public string HallName { get; set; }
        public int? IsAssigned { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
