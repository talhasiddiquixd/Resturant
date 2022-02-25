using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace RestaurantPOS.Helpers
{
    public class Enums
    {
        public enum OrderStatus
        {
            Requested = 1,
            Rejected = 2,
            Preparing = 3,
            Prepared = 4,
            Served = 5,
            Paid = 6
        }

        public enum OrderType
        {
            Online = 1,
            TakeAway = 2,
            Dinning = 3,
            DriveThrough = 4,
            Delivery = 5
        }

    }
}
