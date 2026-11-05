using SecureEDM.Client.Contracts.Interfaces.Repositories.ModelRepo;
using SecureEDM.Common.Contracts.Interfaces;

namespace SecureEDM.Client.Contracts.Interfaces;

public interface IUnitOfWorkClient : IUnitOfWork
{
    IAsymmetricKeyRepo AsymmetricKey { get; }
    IHashOfDocumentRepo HashOfDocument { get; }
    IPrivateKeyRepo PrivateKey { get; }
    IPublicKeyRepo PublicKey { get; }
    ISignatureRepo Signature { get; }
    ITrackingDocumentRepo TrackingDocument { get; }
}