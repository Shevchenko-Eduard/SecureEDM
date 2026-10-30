using System.Numerics;

namespace SecureEDM.Common.Domain.Models.Interfaces;

public interface IHasId<T>
{
    public T Id { get; init; }
}