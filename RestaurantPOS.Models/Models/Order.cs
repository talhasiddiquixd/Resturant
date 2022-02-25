using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Hall")]
        public int? HallId { get; set; }
        [ForeignKey("Table")]
        public int? TableId { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Discount { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ServiceCharges { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DeliveryCharges { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Tax { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAmount { get; set; }
        public string CookingTime { get; set; }
        public bool PaidStatus { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? PreparedTime { get; set; }
        public DateTime? ServedTime { get; set; }
        public DateTime? CompleteTime { get; set; }
        public string OrderStatus { get; set; }
        public int? OrderType { get; set; }
        public bool? IsSynchronized { get; set; }
        public virtual ICollection<OrderItem> OrderItem { get; set; }
    }
}