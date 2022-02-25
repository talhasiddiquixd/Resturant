using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class AttachmentRequestDTO
    {
        public int Id { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
