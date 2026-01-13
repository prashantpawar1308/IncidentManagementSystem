namespace IMS.Data.Entities;

public class Incident
{
    public int IncidentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
    public string UpdatedBy { get; set; } = string.Empty;

    // Navigation property (one Incident → many Documents)
    public ICollection<DocumentMetadata> DocumentMetadata { get; set; }
}