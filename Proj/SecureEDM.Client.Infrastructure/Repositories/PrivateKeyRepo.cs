using SecureEDM.Common.Contracts.Interfaces;
using SecureEDM.Client.Infrastructure.Database;
using SecureEDM.Client.Contracts.DTOs.PrivateKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories;
using SecureEDM.Client.Domain.Models;
using SecureEDM.Client.Infrastructure.Database.ModelOrm;
using Microsoft.EntityFrameworkCore;

namespace SecureEDM.Client.Infrastructure.Repositories;

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

    public Task<PrivateKey> QueryAsync(QueryPrivateKey dto, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}