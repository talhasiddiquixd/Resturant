namespace RestaurantPOS.Helpers.RequestDTO
{
    public class UserRoleRequestDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }
}
