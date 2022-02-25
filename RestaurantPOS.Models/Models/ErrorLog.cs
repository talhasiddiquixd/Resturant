using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RestaurantPOS.Models
{
    public class ErrorLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string LogController { get; set; }
        public string LogAction { get; set; }
        public string LogMessage { get; set; }
        public string LogDetail { get; set; }
        public string ErrorLogDate { get; set; }
        public string ErrorLogTime { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorLine { get; set; }
        public bool? IsSynchronized { get; set; }
    }
}