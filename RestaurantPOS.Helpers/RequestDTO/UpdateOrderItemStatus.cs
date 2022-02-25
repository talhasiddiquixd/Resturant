namespace RestaurantPOS.Helpers.RequestDTO
{
    public class UpdateOrderItemStatus
    {
        public int OrderId { get; set; }
        public int FoodItemId { get; set; }
        public int FoodVarientId { get; set; }
        public string Status { get; set; }
    }
}
