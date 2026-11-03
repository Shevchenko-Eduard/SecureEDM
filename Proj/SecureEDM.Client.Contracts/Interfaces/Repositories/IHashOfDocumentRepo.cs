using SecureEDM.Client.Contracts.DTOs.HashOfDocument;
using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Models;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories;

public interface IHashOfDocumentRepo :
    IHasCreateRepo<CreateHashOfDocument>,
    IHasDeleteRepo<DeleteHashOfDocument>,
    IHasQueryRepo<QueryHashOfDocument, HashOfDocument>,
    IHasRevokeRepo<RevokeHashOfDocument>
{
    Task SetPrimaryAsync(SetPrimaryHashOfDocument dto, CancellationToken token = default);
}