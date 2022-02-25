 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class FoodCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsOffer { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsSynchronized { get; set; }
        public virtual ICollection<FoodCategoryOffer> FoodCategoryOffer { get; set; }
        public virtual ICollection<FoodItem> FoodItem { get; set; }
    }
}