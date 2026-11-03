using SecureEDM.Client.Contracts.DTOs.TrackingDocument;
using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Models;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories;

public interface ITrackingDocumentRepo :
    IHasCreateRepo<CreateTrackingDocument>,
    IHasDeleteRepo<DeleteTrackingDocument>,
    IHasQueryRepo<QueryTrackingDocument, TrackingDocument>,
    IHasSoftDeleteRepo<SoftDeleteTrackingDocument>
{
    Task UpdateAsync(UpdateTrackingDocument dto, CancellationToken token = default);
}