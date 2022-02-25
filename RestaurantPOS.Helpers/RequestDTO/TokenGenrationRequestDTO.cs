using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class TokenGenrationRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? AssignedRole { get; set; }
        public int? AssignedType { get; set; }
        public string  Role{ get; set; }
    }
}
