using SecureEDM.Client.Contracts.DTOs.PrivateKey;
using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Models;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories;

public interface IPrivateKeyRepo :
    IHasCreateRepo<CreatePrivateKey>,
    IHasDeleteRepo<DeletePrivateKey>,
    IHasQueryRepo<QueryPrivateKey, PrivateKey>
{
    
}