using SecureEDM.Client.Contracts.DTOs.PublicKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using SecureEDM.Client.Infrastructure.Repositories.Tools;

namespace SecureEDM.Client.Infrastructure.Repositories;

public class PublicKeyRepo(
    ClientContext context,
    IMapper mapper,
    ILogger<AsymmetricKeyRepo> logger
    ) : IPublicKeyRepo
{
    private readonly ClientContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AsymmetricKeyRepo> _logger = logger;

    public async Task AttestedAsync(AttestedPublicKey dto, CancellationToken token = default)
    {
        var entity = await _context.PublicKeys.SingleAsync(p => p.Id == dto.Id, token);
        entity.Attested(dto.Attestation);
    }

    public async Task CreateAsync(CreatePublicKey dto, CancellationToken token = default)
    {
        var entity = _mapper.To<PublicKeyOrm>().From(dto);
        await _context.PublicKeys.AddAsync(entity, token);
    }

    public async Task DeleteAsync(DeletePublicKey dto, CancellationToken token = default)
    {
        var entity = await _context.PublicKeys.SingleAsync(p => p.Id == dto.Id, token);
        _context.PublicKeys.Remove(entity);
    }

    public async Task<ReadOnlySet<PublicKey>> QueryAsync(QueryPublicKey dto, CancellationToken token = default)
    {
        var query = _context.PublicKeys.AsQueryable();

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.Id, value => q => value.Contains(q.Id));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.AsymmetricKeyId, value => q => value.Contains(q.AsymmetricKeyId));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.RemoteOwnerId, value => q => q.RemoteOwnerId != null && value.Contains((int)q.RemoteOwnerId));

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.HasAttestation, value => q => q.Attestation != null == value);

        QueryTools.ExecuteIfNotNull(ref query, dto.Top, value => query.Take(value));
        QueryTools.ExecuteIfNotNull(ref query, dto.Skip, value => query.Skip(value));

        var res = await query.ToArrayAsync();

        return [.. res.Select(_mapper.To<PublicKey>().From)];
    }
}