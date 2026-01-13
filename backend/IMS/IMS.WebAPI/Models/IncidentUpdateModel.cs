namespace IMS.WebAPI.Models
{
    public class IncidentUpdateModel
    {
        public string? Status { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}