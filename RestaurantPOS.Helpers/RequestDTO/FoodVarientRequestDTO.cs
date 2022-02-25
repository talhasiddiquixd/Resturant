

namespace RestaurantPOS.Helpers.RequestDTO
{
    public  class FoodVarientRequestDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int FoodItemId { get; set; }
        public int? KitchenId { get; set; }
    }
}
