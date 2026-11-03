using SecureEDM.Client.Contracts.DTOs.Signature;
using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Models;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories;

public interface ISignatureRepo :
    IHasCreateRepo<CreateSignature>,
    IHasDeleteRepo<DeleteSignature>,
    IHasQueryRepo<QuerySignature, Signature>,
    IHasRevokeRepo<RevokeSignature>
{
    
}