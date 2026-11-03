using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Contracts.DTOs.HashOfDocument;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
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

    public Task<HashOfDocument> QueryAsync(QueryHashOfDocument dto, CancellationToken token = default)
    {
        throw new NotImplementedException();
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