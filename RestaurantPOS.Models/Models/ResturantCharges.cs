using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantPOS.Models
{
    public class ResturantCharges
    {
        public int Id { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ServiceCharges { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DeliveryCharges { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Tax { get; set; }
        public bool IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsSynchronized { get; set; }
    }

}
