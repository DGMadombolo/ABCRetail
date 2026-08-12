namespace ABCRetail.Models
{
    public class StoredFile
    {
        public string FileName { get; set; } = string.Empty;

        public string FilePath { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}