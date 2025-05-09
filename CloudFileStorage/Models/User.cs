namespace CloudFileStorage.Models
{
    public class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }

        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }


    }
}
