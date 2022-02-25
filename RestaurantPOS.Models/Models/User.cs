using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantPOS.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public int? AssignedRole { get; set; }     ///  
        public int? AssignedType { get; set; } ///  Kitchen/Hall/Counter
        public string Password { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public int UserAttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsDeleted { get; set; }
        public string FcmToken { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool? IsSynchronized { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}