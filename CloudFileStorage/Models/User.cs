namespace CloudFileStorage.Models
{
    public class User
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public required string username { get; set; }
        public required string password { get; set; }
        public UserRole role { get; set; }

        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

        public ICollection<File> Files { get; set; } = new List<File>();


    }
    public enum UserRole
    {
        Admin,
        User
    }
}
