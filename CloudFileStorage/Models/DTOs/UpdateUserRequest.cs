namespace CloudFileStorage.Models.DTOs
{
    public class UpdateUserRequest
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? role { get; set; }
    }
}
