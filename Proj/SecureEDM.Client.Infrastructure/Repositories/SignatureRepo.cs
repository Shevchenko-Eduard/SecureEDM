using SecureEDM.Client.Contracts.DTOs.Signature;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using SecureEDM.Client.Infrastructure.Repositories.Tools;

namespace SecureEDM.Client.Infrastructure.Repositories;

public class SignatureRepo(
    ClientContext context,
    IMapper mapper,
    ILogger<AsymmetricKeyRepo> logger
    ) : ISignatureRepo
{
    private readonly ClientContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AsymmetricKeyRepo> _logger = logger;

    public async Task CreateAsync(CreateSignature dto, CancellationToken token = default)
    {
        var entity = _mapper.To<SignatureOrm>().From(dto);
        await _context.Signatures.AddAsync(entity, token);
    }

    public async Task DeleteAsync(DeleteSignature dto, CancellationToken token = default)
    {
        var entity = await _context.Signatures.SingleAsync(p => p.Id == dto.Id, token);
        _context.Signatures.Remove(entity);
    }

    public async Task<ReadOnlySet<Signature>> QueryAsync(QuerySignature dto, CancellationToken token = default)
    {
        var query = _context.Signatures.AsQueryable();

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.Id, value => q => value.Contains(q.Id));
        QueryTools.ExecuteIfNotNull(ref query, dto.RemoteAuthorId, value => query.Where(q => q.RemoteAuthorId != null && value.Contains((int)q.RemoteAuthorId)));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.DocumentId, value => q => value.Contains(q.DocumentId));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.PublicKeyId, value => q => value.Contains(q.PublicKeyId));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RevokedReason, value => q => value.Contains(q.RevokedReason));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.SignedAlgorithm, value => q => value.Contains(q.SignedAlgorithm));
        
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.SignedAt,
            actionTo: value => q => value >= q.SignedAt,
            actionFrom: value => q => value <= q.SignedAt);
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RevokedAt,
            actionTo: value => q => value >= q.RevokedAt,
            actionFrom: value => q => value <= q.RevokedAt);

        QueryTools.ExecuteIfNotNull(ref query, dto.Top, value => query.Take(value));
        QueryTools.ExecuteIfNotNull(ref query, dto.Skip, value => query.Skip(value));

        var res = await query.ToArrayAsync();

        return [.. res.Select(_mapper.To<Signature>().From)];
    }

    public async Task RevokeAsync(RevokeSignature dto, CancellationToken token = default)
    {
        var entity = await _context.Signatures.SingleAsync(p => p.Id == dto.Id, token);
        if (dto.RevokedAt == null) entity.Revoke(dto.RevokedReason);
        else if (dto.RevokedAt is DateTimeOffset revokedAt) entity.Revoke(dto.RevokedReason, revokedAt);
    }
}