using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class LoginRequestDTO
    {
        public string EmailOrPhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
