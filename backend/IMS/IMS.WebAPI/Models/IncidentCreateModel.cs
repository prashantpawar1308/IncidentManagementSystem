namespace IMS.WebAPI.Models
{
    public class IncidentCreateModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Severity { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        // Optional single attachment when creating
        public IFormFile? Attachment { get; set; }
    }
}