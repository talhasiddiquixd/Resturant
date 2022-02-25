namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class UserRoleResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
    }
}
