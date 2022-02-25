using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.RequestDTO
{
    public class AssignPermissionRequestDTO
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
    }
}
