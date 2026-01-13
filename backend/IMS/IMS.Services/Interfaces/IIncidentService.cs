using IMS.Services.DTO;

namespace IMS.Services.Interfaces
{
    public interface IIncidentService
    {
        Task<IEnumerable<IncidentDto>> GetAllAsync();

        Task<IncidentDto?> GetByIdAsync(int id);

        Task<IncidentDto> CreateAsync(IncidentDto incident);

        Task UpdateAsync(int id, IncidentDto incident);

        Task DeleteAsync(int id);

        Task<DocumentMetadataDto> AddDocumentMetadataAsync(int incidentId, string fileName, string contentType, long size, string blobUrl, string uploadedBy);
    }
}