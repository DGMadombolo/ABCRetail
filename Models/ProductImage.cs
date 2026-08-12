namespace ABCRetail.Models
{
    public class ProductImage
    {
        public string FileName { get; set; } = string.Empty;

        public string BlobName { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; }
    }
}