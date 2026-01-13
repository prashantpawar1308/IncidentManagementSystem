using System.ComponentModel.DataAnnotations;

namespace IMS.Data.Entities
{
    public class DocumentMetadata
    {
        [Key]
        public Guid DocumentId { get; set; }            
        public int IncidentId { get; set; }         

        public string FileName { get; set; }       
        public string ContentType { get; set; }     = string.Empty;
        public long Size { get; set; }              
        public DateTime UploadDate { get; set; }   
        public string UploadedBy { get; set; }
        public string BlobUrl { get; set; }        
        public Incident Incident { get; set; }
    }
}