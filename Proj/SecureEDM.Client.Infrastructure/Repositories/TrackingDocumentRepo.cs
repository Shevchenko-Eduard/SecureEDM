using SecureEDM.Client.Contracts.DTOs.TrackingDocument;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Infrastructure.Repositories;

public class TrackingDocumentRepo(
    ClientContext context,
    IMapper mapper,
    ILogger<AsymmetricKeyRepo> logger
    ) : ITrackingDocumentRepo
{
    private readonly ClientContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AsymmetricKeyRepo> _logger = logger;

    public async Task CreateAsync(CreateTrackingDocument dto, CancellationToken token = default)
    {
        var entity = _mapper.To<TrackingDocumentOrm>().From(dto);
        await _context.TrackingDocuments.AddAsync(entity, token);
    }

    public async Task DeleteAsync(DeleteTrackingDocument dto, CancellationToken token = default)
    {
        var entity = await _context.TrackingDocuments.SingleAsync(d => d.Id == dto.Id, token);
        _context.TrackingDocuments.Remove(entity);
    }

    public Task<TrackingDocument> QueryAsync(QueryTrackingDocument dto, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task SoftDeleteAsync(SoftDeleteTrackingDocument dto, CancellationToken token = default)
    {
        var entity = await _context.TrackingDocuments.SingleAsync(d => d.Id == dto.Id, token);
        if (dto.DeleteAt == null) entity.SoftDelete();
        else if (dto.DeleteAt is DateTimeOffset deleteAt) entity.SoftDelete(deleteAt);
    }

    public async Task UpdateAsync(UpdateTrackingDocument dto, CancellationToken token = default)
    {
        var entity = await _context.TrackingDocuments.SingleAsync(d => d.Id == dto.Id, token);
        if (dto.LastSync is DateTimeOffset lastSync) entity.LastSync = lastSync;
        if (dto.Path is string path) entity.Path = path;
        if (dto.StatusTracking is EnumStatusTracking statusTracking) entity.StatusTracking = statusTracking;
    }
}