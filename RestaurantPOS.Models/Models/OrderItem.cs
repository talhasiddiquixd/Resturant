using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Order")]
        public int OrderId { get; set; }
        [ForeignKey("FoodItem")]
        public int FoodItemId { get; set; }
        [ForeignKey("FoodVarient")]
        public int FoodVarientId { get; set; }
        [ForeignKey("Kitchen")]
        public int KitchenId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Total { get; set; }
        public string Status { get; set; }
        public bool? IsSynchronized { get; set; }
        public FoodItem FoodItem { get; set; }



        
    }
}