namespace IMS.Services.DTO
{
    public class DocumentMetadataDto
    {
        public Guid DocumentId { get; set; }
        public int IncidentId { get; set; }

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadedBy { get; set; }
        public string BlobUrl { get; set; }
        public IncidentDto? Incident { get; set; }
    }
}