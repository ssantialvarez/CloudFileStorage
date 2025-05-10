namespace CloudFileStorage.Models
{
    public class User
    {
        public string id { get; set; }
        public required string username { get; set; }
        public required string password { get; set; }
        public UserRole role { get; set; }

        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }


    }
    public enum UserRole
    {
        Admin,
        User
    }
}
