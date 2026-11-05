using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Contracts.DTOs.PrivateKey;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;
using SecureEDM.Client.Infrastructure.Repositories.Models.Tools;

namespace SecureEDM.Client.Infrastructure.Repositories.Models;

public class PrivateKeyRepo(
    ClientContext context,
    IMapper mapper,
    ILogger<AsymmetricKeyRepo> logger
    ) : IPrivateKeyRepo
{
    private readonly ClientContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<AsymmetricKeyRepo> _logger = logger;

    public async Task CreateAsync(CreatePrivateKey dto, CancellationToken token = default)
    {
        var entity = _mapper.To<PrivateKeyOrm>().From(dto);
        await _context.PrivateKeys.AddAsync(entity, token);
    }

    public async Task DeleteAsync(DeletePrivateKey dto, CancellationToken token = default)
    {
        var entity = await _context.PrivateKeys.SingleAsync(p => p.Id == dto.Id, token);
        _context.PrivateKeys.Remove(entity);
    }

    public async Task<ReadOnlySet<PrivateKey>> QueryAsync(QueryPrivateKey dto, CancellationToken token = default)
    {
        var query = _context.PrivateKeys.AsQueryable();

        QueryTools.ExecuteWhereIfNotNull(ref query, dto.Id, value => q => value.Contains(q.Id));
        QueryTools.ExecuteWhereIfNotNull(ref query, dto.AsymmetricKeyId, value => q => value.Contains(q.AsymmetricKeyId));
        
        QueryTools.ExecuteIfNotNull(ref query, dto.Top, value => query.Take(value));
        QueryTools.ExecuteIfNotNull(ref query, dto.Skip, value => query.Skip(value));

        var res = await query.ToArrayAsync();

        return [.. res.Select(_mapper.To<PrivateKey>().From)];
    }
}