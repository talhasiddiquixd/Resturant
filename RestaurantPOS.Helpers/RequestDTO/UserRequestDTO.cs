using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
   public class UserRequestDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ContactNo { get; set; }
        public int UserAttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public int? AssignedRole { get; set; }
        public int? AssignedType { get; set; }
    }
}
