using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class FcmTokenResponseDTO
    {
        public int UserId { get; set; }
        public int UserType { get; set; }
        public string FcmToken { get; set; }
    }
}
