using SecureEDM.Client.Contracts.DTOs.AsymmetricKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Models;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;

public interface IAsymmetricKeyRepo : 
    IHasCreateRepo<CreateAsymmetricKey>,
    IHasDeleteRepo<DeleteAsymmetricKey>,
    IHasQueryRepo<QueryAsymmetricKey, AsymmetricKey>,
    IHasRevokeRepo<RevokeAsymmetricKey>
{
}