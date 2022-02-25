using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class EmailRequestModel
    {
        public string EmailSubject { get; set; }
        public string EmailTemplate { get; set; }
        public string EmailToName { get; set; }
        public string EmailToEmail { get; set; }
        public string SMSTemplate { get; set; }
    }
}
