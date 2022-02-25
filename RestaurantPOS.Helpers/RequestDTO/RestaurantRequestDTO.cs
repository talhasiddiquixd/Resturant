namespace RestaurantPOS.Helpers.RequestDTO
{
    public class RestaurantRequestDTO
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
    }
}
