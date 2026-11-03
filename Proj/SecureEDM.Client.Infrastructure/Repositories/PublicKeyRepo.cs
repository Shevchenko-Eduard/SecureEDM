using SecureEDM.Client.Contracts.DTOs.PublicKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;

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

    public Task<PublicKey> QueryAsync(QueryPublicKey dto, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}