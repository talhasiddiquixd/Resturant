using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RestaurantPOS.Models
{
    public class Attachment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FileToUpLoad { get; set; }
        public string FileType { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsSynchronized { get; set; }
    }
}