namespace CloudFileStorage.Models.DTOs
{
    public class FileResponse
    {
        public Guid id { get; set; } = Guid.NewGuid();
        public required string fileName { get; set; }
        public required double size { get; set; }
        public required string contentType { get; set; }
        public DateTime uploadedOn { get; set; }
        public Guid UserId { get; set; }

    }
}
