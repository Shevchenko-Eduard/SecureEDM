using SecureEDM.Client.Contracts.Interfaces.Repositories.Base;
using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Contracts.Interfaces.Repositories.SettingRepo;

public interface IDefaulteConflictStrategyRepo : ISettingRepo<EnumConflictStrategy>
{

}