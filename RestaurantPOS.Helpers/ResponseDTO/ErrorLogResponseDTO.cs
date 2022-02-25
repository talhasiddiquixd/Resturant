using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class ErrorLogResponseDTO
    {
        public int LogId { get; set; }
        public string LogController { get; set; }
        public string LogAction { get; set; }
        public string LogMessage { get; set; }
        public string LogDetail { get; set; }
        public string ErrorLogDate { get; set; }
        public string ErrorLogTime { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorLine { get; set; }
        public int CookId { get; set; }
    }
}
