using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Contracts.DTOs.AsymmetricKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using SecureEDM.Client.Infrastructure.Repositories.Models.Tools;
using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Client.Infrastructure.Repositories.Models;

public class AsymmetricKeyRepo(
    ClientContext context,
    IMapper mapper,
    ILogger<AsymmetricKeyRepo> logger
    ) : IAsymmetricKeyRepo
{
    private readonly ClientContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AsymmetricKeyRepo> _logger = logger;

    public async Task CreateAsync(CreateAsymmetricKey dto, CancellationToken token = default)
    {
        var asymmetricKey = _mapper.To<AsymmetricKeyOrm>().From(dto);
        await _context.AsymmetricKeys.AddAsync(asymmetricKey, token);
    }

    public async Task DeleteAsync(DeleteAsymmetricKey dto, CancellationToken token = default)
    {
        var entity = await _context.AsymmetricKeys.SingleAsync(a => a.Id == dto.Id, token);
        _context.AsymmetricKeys.Remove(entity);
    }

    public async Task<ReadOnlySet<AsymmetricKey>> QueryAsync(QueryAsymmetricKey dto, CancellationToken token = default)
    {
        var query = _context.AsymmetricKeys.AsQueryable();

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.Id, value => q => value.Contains(q.Id));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RevokedReason, value => q => value.Contains(q.RevokedReason));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.KeygenAlgorithm, value => q => value.Contains(q.KeygenAlgorithm));

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.CreatedAt,
            actionTo: value => q => value >= q.CreatedAt,
            actionFrom: value => q => value <= q.CreatedAt);
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RevokedAt,
            actionTo: value => q => value >= q.RevokedAt,
            actionFrom: value => q => value <= q.RevokedAt);

        QueryTools.ExecuteIfNotNull(ref query, dto.Top, value => query.Take(value));
        QueryTools.ExecuteIfNotNull(ref query, dto.Skip, value => query.Skip(value));

        var res = await query.ToArrayAsync();

        return [.. res.Select(_mapper.To<AsymmetricKey>().From)];
    }

    public async Task RevokeAsync(RevokeAsymmetricKey dto, CancellationToken token = default)
    {
        var entity = await _context.AsymmetricKeys.SingleAsync(a => a.Id == dto.Id, token);
        if (dto.RevokedAt == null) entity.Revoke(dto.RevokedReason);
        else if (dto.RevokedAt is DateTimeOffset revokedAt) entity.Revoke(dto.RevokedReason, revokedAt);
    }
}