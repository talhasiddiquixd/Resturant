namespace RestaurantPOS.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string RestaurantName { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public int AttachmentId { get; set; }
        public string AttachmentPath { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsSynchronized { get; set; }
    }
}
