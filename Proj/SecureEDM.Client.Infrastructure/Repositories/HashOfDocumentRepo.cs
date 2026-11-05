using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Contracts.DTOs.HashOfDocument;
using SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using SecureEDM.Client.Infrastructure.Repositories.Tools;
using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Client.Infrastructure.Repositories;

public class HashOfDocumentRepo(
    ClientContext context,
    IMapper mapper,
    ILogger<AsymmetricKeyRepo> logger
    ) : IHashOfDocumentRepo
{
    private readonly ClientContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AsymmetricKeyRepo> _logger = logger;

    public async Task CreateAsync(CreateHashOfDocument dto, CancellationToken token = default)
    {
        var entity = _mapper.To<HashOfDocumentOrm>().From(dto);
        await _context.HashOfDocuments.AddAsync(entity, token);
    }

    public async Task DeleteAsync(DeleteHashOfDocument dto, CancellationToken token = default)
    {
        var entity = await _context.HashOfDocuments.SingleAsync(a => a.Id == dto.Id, token);
        _context.HashOfDocuments.Remove(entity);
    }

    public async Task<ReadOnlySet<HashOfDocument>> QueryAsync(QueryHashOfDocument dto, CancellationToken token = default)
    {
        var query = _context.HashOfDocuments.AsQueryable();

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.Id, value => q => value.Contains(q.Id));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RemoteAuthorId, value => q => q.RemoteAuthorId != null && value.Contains((int)q.RemoteAuthorId));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RevokedReason, value => q => value.Contains(q.RevokedReason));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.HashedAlgorithm, value => q => value.Contains(q.HashedAlgorithm));

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.CreatedAt,
            actionTo: value => q => value >= q.CreatedAt,
            actionFrom: value => q => value <= q.CreatedAt);
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RevokedAt,
            actionTo: value => q => value >= q.RevokedAt,
            actionFrom: value => q => value <= q.RevokedAt);

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.IsPrimary, value => q => q.IsPrimary == value);
        
        QueryTools.ExecuteIfNotNull(ref query, dto.Top, value => query.Take(value));
        QueryTools.ExecuteIfNotNull(ref query, dto.Skip, value => query.Skip(value));

        var res = await query.ToArrayAsync();

        return [.. res.Select(_mapper.To<HashOfDocument>().From)];
    }

    public async Task RevokeAsync(RevokeHashOfDocument dto, CancellationToken token = default)
    {
        var entity = await _context.HashOfDocuments.SingleAsync(a => a.Id == dto.Id, token);
        if (dto.RevokedAt == null) entity.Revoke(dto.RevokedReason);
        else if (dto.RevokedAt is DateTimeOffset revokedAt) entity.Revoke(dto.RevokedReason, revokedAt);
    }

    public async Task SetPrimaryAsync(SetPrimaryHashOfDocument dto, CancellationToken token = default)
    {
        var entity = await _context.HashOfDocuments.SingleAsync(a => a.Id == dto.Id, token);
        var primaryEntity = _context.HashOfDocuments
            .Where(d => d.DocumentId == entity.DocumentId && d.IsPrimary == true);
        await primaryEntity.ForEachAsync(d => d.SetPrimary(false));
        entity.SetPrimary(true);
    }
}