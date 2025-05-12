namespace CloudFileStorage.Models
{
    public class File
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public required string fileName { get; set; }
        public required double size { get; set; }

        public required string contentType { get; set; }
        public DateTime uploadedOn { get; set; }

        public User User { get; set; }
        public Guid UserId { get; set; }

    }
}
