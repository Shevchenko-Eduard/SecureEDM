using Microsoft.EntityFrameworkCore;
using SecureEDM.Client.Contracts.DTOs.AsymmetricKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Client.Infrastructure.Repositories;

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

    public Task<AsymmetricKey> QueryAsync(QueryAsymmetricKey dto, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task RevokeAsync(RevokeAsymmetricKey dto, CancellationToken token = default)
    {
        var entity = await _context.AsymmetricKeys.SingleAsync(a => a.Id == dto.Id, token);
        if (dto.RevokedAt == null) entity.Revoke(dto.RevokedReason);
        else if (dto.RevokedAt is DateTimeOffset revokedAt) entity.Revoke(dto.RevokedReason, revokedAt);
    }
}