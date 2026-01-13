using IMS.Services;
using IMS.Services.DTO;
using IMS.Services.Interfaces;
using IMS.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace IMS.WebAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class IncidentController : ControllerBase
    {
        private readonly IIncidentService _incidentService;
        private readonly IBlobStorageService? _blobStorageService;
        private readonly ILogger<IncidentController> _logger;

        public IncidentController(IIncidentService incidentService,
                                   IBlobStorageService? blobStorageService,
                                   ILogger<IncidentController> logger)
        {
            _incidentService = incidentService;
            _blobStorageService = blobStorageService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<IncidentModel>> Get()
        {
            var dtos = await _incidentService.GetAllAsync().ConfigureAwait(false);
            return dtos?.Select(dto => new IncidentModel
            {
                Id = dto?.IncidentId ?? 0,
                Title = dto?.Title ?? string.Empty,
                Description = dto?.Description ?? string.Empty,
                Severity = dto?.Severity ?? string.Empty,
                Status = dto?.Status ?? string.Empty,
                CreatedAt = dto?.CreatedAt ?? DateTime.MinValue,
                CreatedBy = dto?.CreatedBy ?? string.Empty,
                UpdatedAt = dto?.UpdatedAt ?? DateTime.MinValue,
                UpdatedBy = dto?.UpdatedBy ?? string.Empty,
                AttachmentPath= dto?.AttachmentUrl ?? string.Empty
            }) ?? Enumerable.Empty<IncidentModel>();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IncidentModel>> Get(int id)
        {
            var dto = await _incidentService.GetByIdAsync(id).ConfigureAwait(false);
            if (dto == null)
            {
                return NotFound();
            }

            var model = new IncidentModel
            {
                Id = dto.IncidentId ?? 0,
                Title = dto.Title ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                Severity = dto.Severity ?? string.Empty,
                Status = dto.Status ?? string.Empty,
                CreatedAt = dto.CreatedAt ?? DateTime.MinValue,
                CreatedBy = dto.CreatedBy ?? string.Empty,
                UpdatedAt = dto.UpdatedAt ?? DateTime.MinValue,
                UpdatedBy = dto.UpdatedBy ?? string.Empty
            };

            return Ok(model);
        }

        [HttpPost]
        public async Task<ActionResult<IncidentModel>> Post([FromForm] IncidentCreateModel Incident)
        {
            if (Incident == null)
            {
                return BadRequest();
            }

            var dto = new IncidentDto
            {
                Title = Incident.Title,
                Description = Incident.Description,
                Severity = Incident.Severity,
                Status = Incident.Status,
                CreatedAt = Incident.CreatedAt,
                CreatedBy = Incident.CreatedBy
            };

            var created = await _incidentService.CreateAsync(dto);
            if (created == null || created.IncidentId == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            _logger.LogInformation("Starting Uploading.");
            // If a file was attached, upload to blob storage and persist metadata
            List<DocumentMetadataModel> attachments = new List<DocumentMetadataModel>();
            if (Incident.Attachment != null && Incident.Attachment.Length > 0 && _blobStorageService != null)
            {
                _logger.LogInformation("Uploading attachment to blob storage.");
                var container = "ims-attachment";
                var fileName = Path.GetFileName(Incident.Attachment.FileName);
                var blobName = $"{created.IncidentId}/{Guid.NewGuid()}_{fileName}";
                using var stream = Incident.Attachment.OpenReadStream();

                var blobUrl = await _blobStorageService.UploadFileAsync(stream, container, blobName, Incident.Attachment.ContentType ?? "application/octet-stream").ConfigureAwait(false);
                _logger.LogInformation($"UblobUrl {blobUrl}");

                var sasUri = await _blobStorageService.CreateServiceSASBlobUrl(container, blobName, string.Empty).ConfigureAwait(false);
                _logger.LogInformation("sasUri Generated.");
                var doc = await _incidentService.AddDocumentMetadataAsync(created.IncidentId.Value, fileName, Incident.Attachment.ContentType ?? string.Empty, Incident.Attachment.Length, sasUri, created.CreatedBy ?? "System").ConfigureAwait(false);
                _logger.LogInformation("Added Metadata.");

                attachments.Add(new DocumentMetadataModel
                {
                    DocumentId = doc.DocumentId,
                    IncidentId = doc.IncidentId,
                    FileName = doc.FileName,
                    ContentType = doc.ContentType,
                    Size = doc.Size,
                    UploadDate = doc.UploadDate,
                    UploadedBy = doc.UploadedBy,
                    BlobUrl = doc.BlobUrl
                });
            }

            var model = new IncidentModel
            {
                Id = created.IncidentId.Value,
                Title = created.Title ?? string.Empty,
                Description = created.Description ?? string.Empty,
                Severity = created.Severity ?? string.Empty,
                Status = created.Status ?? string.Empty,
                CreatedAt = created.CreatedAt ?? DateTime.MinValue,
                CreatedBy = created.CreatedBy ?? string.Empty,
                UpdatedAt = created.UpdatedAt ?? DateTime.MinValue,
                UpdatedBy = created.UpdatedBy ?? string.Empty
            };

            if (attachments.Any())
            {
                model.AttachmentPath = attachments.First().BlobUrl;
            }

            return CreatedAtAction(nameof(Get), new { id = created.IncidentId.Value }, model);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] IncidentUpdateModel Incident)
        {

            if (Incident == null)
            {
                return BadRequest();
            }

            var existingData = await _incidentService.GetByIdAsync(id);
            try
            {
                existingData?.Status = Incident.Status ?? existingData?.Status;
                existingData?.UpdatedBy = Incident.UpdatedBy ?? existingData?.UpdatedBy;
                existingData?.UpdatedAt = Incident.UpdatedAt;
                if (existingData != null)
                {
                    await _incidentService.UpdateAsync(id, existingData).ConfigureAwait(false);
                }
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _incidentService.DeleteAsync(id).ConfigureAwait(false);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}