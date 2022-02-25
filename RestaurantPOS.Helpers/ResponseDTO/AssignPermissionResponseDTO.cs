using System;
using System.Collections.Generic;
using System.Text;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class AssignPermissionResponseDTO
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public string PermissionName { get; set; }
        public string RoleName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public PermissionResponseDTO Premission { get; set; }
    }
}
