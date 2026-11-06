using SecureEDM.Client.Application.Models;
using SecureEDM.Client.Domain.Enums;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Presentation.ViewModels;

public sealed class TemplateOption(string key, string name, string title, string body, bool hasTemplate)
{
    public string Key { get; } = key;
    public string Name { get; } = name;
    public string Title { get; } = title;
    public string Body { get; } = body;
    public bool HasTemplate { get; } = hasTemplate;
    public override string ToString() => Name;
}

public sealed class KeyAlgorithmOption(
    EnumKpgAlgorithm algorithm, string name, string description,
    int defaultLength, IReadOnlyList<int> lengths, bool supportsSigning)
{
    public EnumKpgAlgorithm Algorithm { get; } = algorithm;
    public string Name { get; } = name;
    public string Description { get; } = description;
    public int DefaultLength { get; } = defaultLength;
    public IReadOnlyList<int> Lengths { get; } = lengths;
    public bool SupportsSigning { get; } = supportsSigning;
    public override string ToString() => Name;
}

public sealed class SignatureAlgorithmOption(EnumSignedAlgorithm algorithm, string name)
{
    public EnumSignedAlgorithm Algorithm { get; } = algorithm;
    public string Name { get; } = name;
    public override string ToString() => Name;
}

public sealed class PageSizeOption(int value)
{
    public int Value { get; } = value;
    public string Label => Value.ToString();
    public override string ToString() => Label;
}

public sealed class DocumentOption(string id, string display)
{
    public string Id { get; } = id;
    public string Display { get; } = display;
    public bool IsReal => !string.IsNullOrEmpty(Id);
    public override string ToString() => Display;
}

public sealed class KeyOption(string id, string display)
{
    public string Id { get; } = id;
    public string Display { get; } = display;
    public bool IsReal => !string.IsNullOrEmpty(Id);
    public override string ToString() => Display;
}

public sealed class SignatureOption(string id, string display)
{
    public string Id { get; } = id;
    public string Display { get; } = display;
    public bool IsReal => !string.IsNullOrEmpty(Id);
    public override string ToString() => Display;
}

public sealed class DocumentRow(
    string id, string number, string title, string author, string created, string createdFull,
    string hashShort, string hashFull, string statusKind, string statusText, string statusFull,
    bool isSelected)
{
    public string Id { get; } = id;
    public string Number { get; } = number;
    public string Title { get; } = title;
    public string Author { get; } = author;
    public string Created { get; } = created;
    public string CreatedFull { get; } = createdFull;
    public string HashShort { get; } = hashShort;
    public string HashFull { get; } = hashFull;
    public string StatusKind { get; } = statusKind;
    public string StatusText { get; } = statusText;
    public string StatusFull { get; } = statusFull;
    public bool IsSelected { get; } = isSelected;
    public bool IsOk => StatusKind == "ok";
    public bool IsWarn => StatusKind == "warn";
    public bool IsNone => StatusKind == "none";
    public bool IsBad => StatusKind == "bad";
}

public sealed class KeyRow(
    string id, string owner, string algorithmLabel, string validUntil, string validUntilFull,
    string fingerprintShort, string fingerprintFull, string statusText,
    bool isActive, bool isExpired, bool isRevoked, bool inSession, bool isSelected)
{
    public string Id { get; } = id;
    public string Owner { get; } = owner;
    public string AlgorithmLabel { get; } = algorithmLabel;
    public string ValidUntil { get; } = validUntil;
    public string ValidUntilFull { get; } = validUntilFull;
    public string FingerprintShort { get; } = fingerprintShort;
    public string FingerprintFull { get; } = fingerprintFull;
    public string StatusText { get; } = statusText;
    public bool IsActive { get; } = isActive;
    public bool IsExpired { get; } = isExpired;
    public bool IsRevoked { get; } = isRevoked;
    public bool InSession { get; } = inSession;
    public bool NotInSession => !InSession;
    public string SessionText => InSession ? "в сессии" : "нет";
    public bool IsSelected { get; } = isSelected;
}

public sealed class SignatureRow(
    string id, string time, string documentTitle, string signer, string algorithmLabel,
    string hashShort, string hashFull, string statusText, string statusFull,
    bool isOk, bool isRevoked)
{
    public string Id { get; } = id;
    public string Time { get; } = time;
    public string DocumentTitle { get; } = documentTitle;
    public string Signer { get; } = signer;
    public string AlgorithmLabel { get; } = algorithmLabel;
    public string HashShort { get; } = hashShort;
    public string HashFull { get; } = hashFull;
    public string StatusText { get; } = statusText;
    public string StatusFull { get; } = statusFull;
    public bool IsOk { get; } = isOk;
    public bool IsRevoked { get; } = isRevoked;
    public bool IsBad => !IsOk;
}

public sealed class LogRow(string time, string user, string type, string message)
{
    public string Time { get; } = time;
    public string User { get; } = user;
    public string Type { get; } = type;
    public string Message { get; } = message;
}

public sealed class HashAlgorithmOption(EnumDigestAlgorithm algorithm, string name)
{
    public EnumDigestAlgorithm Algorithm { get; } = algorithm;
    public string Name { get; } = name;
    public override string ToString() => Name;
}

public sealed class RevocationReasonOption(EnumRevokedReason reason, string name)
{
    public EnumRevokedReason Reason { get; } = reason;
    public string Name { get; } = name;
    public override string ToString() => Name;
}

public sealed class ConflictStrategyOption(EnumConflictStrategy strategy, string name)
{
    public EnumConflictStrategy Strategy { get; } = strategy;
    public string Name { get; } = name;
    public override string ToString() => Name;
}

public sealed class DocumentHistoryRow(
    int version, string title, string hashShort, string archivedAt)
{
    public int Version { get; } = version;
    public string Title { get; } = title;
    public string HashShort { get; } = hashShort;
    public string ArchivedAt { get; } = archivedAt;
}

public sealed class HashRow(
    string id, string algorithmLabel, string hashShort, string hashFull,
    bool isPrimary, string statusKind, string statusText, string statusFull,
    string created, bool isSelected)
{
    public string Id { get; } = id;
    public string AlgorithmLabel { get; } = algorithmLabel;
    public string HashShort { get; } = hashShort;
    public string HashFull { get; } = hashFull;
    public bool IsPrimary { get; } = isPrimary;
    public bool IsRevoked { get; } = statusKind == "bad";
    public bool IsAdditional => !IsPrimary && !IsRevoked;
    public bool CanSetPrimary => !IsPrimary && !IsRevoked;
    public string StatusKind { get; } = statusKind;
    public string StatusText { get; } = statusText;
    public string StatusFull { get; } = statusFull;
    public string Created { get; } = created;
    public bool IsSelected { get; } = isSelected;
}

public sealed class TrackingFileRow(
    string id, string path, string extension, string statusKind, string statusText,
    string statusFull, string created, string lastSync, bool isDeleted, bool isSelected)
{
    public string Id { get; } = id;
    public string Path { get; } = path;
    public string Extension { get; } = extension;
    public string StatusKind { get; } = statusKind;
    public string StatusText { get; } = statusText;
    public string StatusFull { get; } = statusFull;
    public string Created { get; } = created;
    public string LastSync { get; } = lastSync;
    public bool IsDeleted { get; } = isDeleted;
    public bool IsSelected { get; } = isSelected;
    public bool CanAct => !IsDeleted;
    public bool IsActive => StatusKind == "ok";
    public bool IsWarn => StatusKind == "warn";
    public bool IsBad => StatusKind == "bad";
    public bool IsNone => StatusKind == "none";
}

public sealed class ToastViewModel(string message, string kind)
{
    public string Message { get; } = message;
    public bool IsOk => kind == "ok";
    public bool IsBad => kind == "bad";
    public bool IsWarn => kind == "warn";
    public bool IsNeutral => kind is not ("ok" or "bad" or "warn");
}
