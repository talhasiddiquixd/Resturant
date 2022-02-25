using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class FoodVarient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("FoodItems")]
        public int FoodItemId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        public int? KitchenId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime?  UpdatedAt { get; set; }
        public int  UpdatedBy { get; set; }
        public int? CreatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsSynchronized { get; set; }
    }
}