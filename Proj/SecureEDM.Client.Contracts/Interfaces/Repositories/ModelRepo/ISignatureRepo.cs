using SecureEDM.Client.Contracts.DTOs.Signature;
using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Models;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;

public interface ISignatureRepo :
    IHasCreateRepo<CreateSignature>,
    IHasDeleteRepo<DeleteSignature>,
    IHasQueryRepo<QuerySignature, Signature>,
    IHasRevokeRepo<RevokeSignature>
{
    
}