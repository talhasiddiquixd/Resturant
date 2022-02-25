namespace RestaurantPOS.Helpers.ResponseDTO
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? AssignedRole { get; set; }
        public int? AssignedType { get; set; }
        public string Role { get; set; }
    }
}
