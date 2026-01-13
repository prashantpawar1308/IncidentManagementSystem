namespace IMS.WebAPI.Models
{
    public class DocumentMetadataModel
    {
        public Guid DocumentId { get; set; }
        public int IncidentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; } = string.Empty;
        public string BlobUrl { get; set; } = string.Empty;
    }
}