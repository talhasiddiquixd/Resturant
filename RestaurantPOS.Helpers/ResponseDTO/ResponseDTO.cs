using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class ResponseDTO<T>
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public int TotalRecords { get; set; }
        public T Data { get; set; }
        public string Message { get; set; }
        public string ExceptionMessage { get; set; }
    }
}
