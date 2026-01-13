using IMS.Data.Entities;
using IMS.Data.Interfaces;
using IMS.Services.DTO;
using IMS.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IMS.Services.Services;

public class IncidentService : IIncidentService
{
    private readonly IGenericRepository<Incident> _repo;
    private readonly IGenericRepository<IMS.Data.Entities.DocumentMetadata> _docRepo;

    public IncidentService(IGenericRepository<Incident> repo, IGenericRepository<IMS.Data.Entities.DocumentMetadata> docRepo)
    {
        _repo = repo;
        _docRepo = docRepo;
    }

    public async Task<IEnumerable<IncidentDto>> GetAllAsync()
    {
        var entities = await _repo.Query()
                                  .Include(i => i.DocumentMetadata)
                                  .ToListAsync()
                                  .ConfigureAwait(false);
        return entities.Select(MapToDto).ToList();
    }

    public async Task<IncidentDto?> GetByIdAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<IncidentDto> CreateAsync(IncidentDto incident)
    {
        var entity = new Incident
        {
            Title = incident.Title ?? string.Empty,
            Description = incident.Description ?? string.Empty,
            Severity = incident.Severity ?? string.Empty,
            Status = incident.Status ?? string.Empty,
            CreatedAt = incident.CreatedAt ?? DateTime.UtcNow,
            CreatedBy = incident.CreatedBy ?? "System",
            UpdatedAt = incident.UpdatedAt ?? DateTime.UtcNow,
            UpdatedBy = incident.UpdatedBy ?? (incident.CreatedBy ?? "System"),
        };

        await _repo.AddAsync(entity).ConfigureAwait(false);
        await _repo.SaveChangesAsync().ConfigureAwait(false);

        incident.IncidentId = entity.IncidentId;
        return incident;
    }

    public async Task<DocumentMetadataDto> AddDocumentMetadataAsync(int incidentId, string fileName, string contentType, long size, string blobUrl, string uploadedBy)
    {
        var doc = new DocumentMetadata
        {
            DocumentId = Guid.NewGuid(),
            IncidentId = incidentId,
            FileName = fileName,
            ContentType = contentType ?? string.Empty,
            Size = size,
            UploadDate = DateTime.UtcNow,
            UploadedBy = uploadedBy ?? "System",
            BlobUrl = blobUrl
        };

        await _docRepo.AddAsync(doc).ConfigureAwait(false);
        await _docRepo.SaveChangesAsync().ConfigureAwait(false);

        return new DocumentMetadataDto
        {
            DocumentId = doc.DocumentId,
            IncidentId = doc.IncidentId,
            FileName = doc.FileName,
            ContentType = doc.ContentType,
            Size = doc.Size,
            UploadDate = doc.UploadDate,
            UploadedBy = doc.UploadedBy,
            BlobUrl = doc.BlobUrl,
            Incident = null
        };
    }

    public async Task UpdateAsync(int id, IncidentDto incident)
    {
        var entity = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        if (entity == null)
            throw new KeyNotFoundException($"Incident {id} not found");

        entity.Title = incident.Title ?? entity.Title;
        entity.Description = incident.Description ?? entity.Description;
        entity.Severity = incident.Severity ?? entity.Severity;
        entity.Status = incident.Status ?? entity.Status;
        entity.UpdatedAt = incident.UpdatedAt ?? DateTime.UtcNow;
        entity.UpdatedBy = incident.UpdatedBy ?? entity.UpdatedBy;

        await _repo.UpdateAsync(entity).ConfigureAwait(false);
        await _repo.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repo.GetByIdAsync(id).ConfigureAwait(false);
        if (entity == null)
            throw new KeyNotFoundException($"Incident {id} not found");

        await _repo.DeleteAsync(entity).ConfigureAwait(false);
        await _repo.SaveChangesAsync().ConfigureAwait(false);
    }

    private static IncidentDto MapToDto(Incident e) => new IncidentDto
    {
        IncidentId = e.IncidentId,
        Title = e.Title,
        Description = e.Description,
        Severity = e.Severity,
        Status = e.Status,
        CreatedAt = e.CreatedAt,
        CreatedBy = e.CreatedBy,
        UpdatedAt = e.UpdatedAt,
        UpdatedBy = e.UpdatedBy,
        AttachmentUrl = e?.DocumentMetadata?.FirstOrDefault()?.BlobUrl ?? string.Empty
    };
}