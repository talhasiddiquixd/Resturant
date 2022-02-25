using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class FoodItemOffer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }
        public string OfferName { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OfferPrice { get; set; }
        public DateTime? OfferStart { get; set; }
        public DateTime? OfferEnd { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSynchronized { get; set; }
    }
}