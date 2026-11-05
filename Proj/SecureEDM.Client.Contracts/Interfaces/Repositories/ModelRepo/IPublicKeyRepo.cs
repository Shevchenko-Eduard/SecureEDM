using SecureEDM.Client.Contracts.DTOs.PublicKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Models;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;

public interface IPublicKeyRepo :
    IHasCreateRepo<CreatePublicKey>,
    IHasDeleteRepo<DeletePublicKey>,
    IHasQueryRepo<QueryPublicKey, PublicKey>
{
    Task AttestedAsync(AttestedPublicKey dto, CancellationToken token = default);
}