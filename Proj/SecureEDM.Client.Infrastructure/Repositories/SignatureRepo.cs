using SecureEDM.Client.Contracts.DTOs.Signature;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;

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

    public Task<Signature> QueryAsync(QuerySignature dto, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task RevokeAsync(RevokeSignature dto, CancellationToken token = default)
    {
        var entity = await _context.Signatures.SingleAsync(p => p.Id == dto.Id, token);
        if (dto.RevokedAt == null) entity.Revoke(dto.RevokedReason);
        else if (dto.RevokedAt is DateTimeOffset revokedAt) entity.Revoke(dto.RevokedReason, revokedAt);
    }
}