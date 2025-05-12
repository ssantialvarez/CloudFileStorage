namespace CloudFileStorage.Models.DTOs
{
    public class S3ObjectDto
    {
        public string? fileName { get; set; }
        public string? presignedUrl { get; set; }
    }
}
