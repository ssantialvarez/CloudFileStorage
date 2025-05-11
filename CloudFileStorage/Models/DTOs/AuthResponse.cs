namespace CloudFileStorage.Models.DTOs
{
    public class AuthResponse
    {
        public bool success { get; set; }
        public string? token { get; set; }
        public string? message { get; set; }

    }
}
