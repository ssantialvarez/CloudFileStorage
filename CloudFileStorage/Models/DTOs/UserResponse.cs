namespace CloudFileStorage.Models.DTOs
{
    public class UserResponse
    {
        public string id { get; set; }
        public string username { get; set; }
        public UserRole role { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
}
