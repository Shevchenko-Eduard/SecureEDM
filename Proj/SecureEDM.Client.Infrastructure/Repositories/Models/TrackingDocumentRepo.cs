using SecureEDM.Client.Contracts.DTOs.TrackingDocument;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Domain.Enums;
using System.Collections.ObjectModel;
using SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;
using SecureEDM.Client.Infrastructure.Repositories.Models.Tools;

namespace SecureEDM.Client.Infrastructure.Repositories.Models;

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

    public async Task<ReadOnlySet<TrackingDocument>> QueryAsync(QueryTrackingDocument dto, CancellationToken token = default)
    {
        var query = _context.TrackingDocuments.AsQueryable();

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.Id, value => q => value.Contains(q.Id));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.Path, value => q => value.Contains(q.Path));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.StatusTracking, value => q => value.Contains(q.StatusTracking));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.FixedExtension, value => q => value.Contains(q.FixedExtension));

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.CreatedAt,
            actionTo: value => q => value >= q.CreatedAt,
            actionFrom: value => q => value <= q.CreatedAt);
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.DeleteAt,
            actionTo: value => q => value >= q.DeleteAt,
            actionFrom: value => q => value <= q.DeleteAt);
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.LastSync,
            actionTo: value => q => value >= q.LastSync,
            actionFrom: value => q => value <= q.LastSync);


        QueryTools.ExecuteIfNotNull(ref query, dto.Top, value => query.Take(value));
        QueryTools.ExecuteIfNotNull(ref query, dto.Skip, value => query.Skip(value));

        var res = await query.ToArrayAsync();

        return [.. res.Select(_mapper.To<TrackingDocument>().From)];
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