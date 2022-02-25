using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class UserLoginRequestDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
