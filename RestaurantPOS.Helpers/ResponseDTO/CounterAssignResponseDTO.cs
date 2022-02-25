using System;
namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class CounterAssignResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public  string UserName {get;set;}
        public int CounterId { get; set; }
        public string CounterName { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
