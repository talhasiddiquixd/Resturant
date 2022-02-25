namespace RestaurantPOS.Helpers.RequestDTO
{
    public class AssignRoleToUserRequestDTO
    {
        public int UserId { get; set; }
        public int? AssignedRole { get; set; }
        public int? AssignedType { get; set; }
    }
}
