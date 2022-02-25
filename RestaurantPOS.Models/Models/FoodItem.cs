using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class FoodItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Note { get; set; }
        public string VAT { get; set; }
        [ForeignKey("FoodCategory")]
        public int FoodCategoryId { get; set; }
        [ForeignKey("Kitchen")]
        public int KitchenId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string CookingTime { get; set; }
        public int Quantity  { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsOffer { get; set; }
        public bool IsSpecial { get; set; }
        public bool Status { get; set; }
        public bool? IsSynchronized { get; set; }
        public virtual ICollection<FoodItemOffer> FoodItemOffer { get; set; }
        public virtual ICollection<FoodVarient> FoodVarient { get; set; }
    }
}