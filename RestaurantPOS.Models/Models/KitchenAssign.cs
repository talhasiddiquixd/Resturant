using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class KitchenAssign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Kitchen")]
        public int KitchenId { get; set; }
        public DateTime? AssignDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsSynchronized { get; set; }

    }
}