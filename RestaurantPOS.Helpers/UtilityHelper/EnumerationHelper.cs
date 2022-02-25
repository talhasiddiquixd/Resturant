using RestaurantPOS.Helpers.ResponseDTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantPOS.Helpers.UtilityHelper
{
    public class EnumerationHelper
    {
            public static EnumResponseDTO GetEnumsList()
            {
                var response = new EnumResponseDTO();
                /// A dictionnary of<string, int>
                Dictionary<string, int> myDic = new Dictionary<string, int>();
                response.OrderTypes = ((Enums.OrderType[])Enum.GetValues(typeof(Enums.OrderType))).ToDictionary(k => k.ToString(), v => (int)v);
                response.OrderStatus = ((Enums.OrderStatus[])Enum.GetValues(typeof(Enums.OrderStatus))).ToDictionary(k => k.ToString(), v => (int)v);
                return response;
            }
    }
}
