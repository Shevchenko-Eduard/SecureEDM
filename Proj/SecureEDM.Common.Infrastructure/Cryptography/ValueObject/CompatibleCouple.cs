namespace SecureEDM.Common.Infrastructure.Cryptography.ValueObject;

public struct CompatibleCouple<TLeft, TRight>(TLeft left, TRight right) 
    where TLeft : struct 
    where TRight : struct
{
    public TLeft Left = left;
    public TRight Right = right;
}