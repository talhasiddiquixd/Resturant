using System;
using System.Collections.Generic;

namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ContactNo { get; set; }
        public int UserAttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public string Attachment { get; set; }
        public int? AssignedRole { get; set; }
        public int? AssignedType { get; set; }
        public string AssignesTypeName { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public List<UserRoleResponseDTO> UserRoles { get; set; }

    }
}
