namespace CloudFileStorage.Models.DTOs
{
    public class UserResponse
    {
        public UserResponse(string _id, string _username, UserRole _role, DateTime _createdAt, DateTime _updatedAt)
        {
            id = _id;
            username = _username;
            role = _role;
            createdAt = _createdAt;
            updatedAt = _updatedAt;
        }
        public string id { get; set; }
        public string username { get; set; }
        public UserRole role { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }

    }
}
