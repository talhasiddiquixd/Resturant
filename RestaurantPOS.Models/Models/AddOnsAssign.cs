using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantPOS.Models
{
    public class AddOnsAssign
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("AddOns")]
        public int AddOnsId { get; set; }
        [ForeignKey("FoodVarient")]
        public int FoodVarientId { get; set; }
        public bool? IsSynchronized { get; set; }
    }
}