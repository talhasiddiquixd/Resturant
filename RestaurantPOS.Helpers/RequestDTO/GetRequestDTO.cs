using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class GetRequestDTO
    {
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
    }
}
  