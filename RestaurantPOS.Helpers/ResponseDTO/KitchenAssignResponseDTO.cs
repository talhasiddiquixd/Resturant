using System;
namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class KitchenAssignResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int KitchenId { get; set; }
        public DateTime? AssignDate { get; set; }
        public string UserName { get; set; }
        public string KitchenName { get; set; }


    }
}
