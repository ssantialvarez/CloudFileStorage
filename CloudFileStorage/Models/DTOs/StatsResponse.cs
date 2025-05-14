namespace CloudFileStorage.Models.DTOs
{
    public class StatsResponse
    {
        public double size { get; set; }
        public UserResponse user { get; set; }
        public StatsResponse(double _size, UserResponse _user)
        {
            size = _size;
            user = _user;
        }
        
    }
}
