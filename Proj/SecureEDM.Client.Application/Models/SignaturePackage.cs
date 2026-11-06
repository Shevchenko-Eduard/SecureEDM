using System.Text.Json.Serialization;

namespace SecureEDM.Client.Application.Models;

/// <summary>Пакет подписи edo-signature/2, пригодный для обмена и независимой проверки.</summary>
public sealed class SignaturePackage
{
    [JsonPropertyName("format")]
    public string Format { get; set; } = "edo-signature/2";

    [JsonPropertyName("document")]
    public PackageDocument Document { get; set; } = new();

    [JsonPropertyName("signature")]
    public PackageSignature Signature { get; set; } = new();

    [JsonPropertyName("publicKey")]
    public PackagePublicKey PublicKey { get; set; } = new();
}

public sealed class PackageDocument
{
    [JsonPropertyName("number")]
    public string Number { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("sha256")]
    public string Sha256 { get; set; } = string.Empty;
}

public sealed class PackageSignature
{
    [JsonPropertyName("algorithm")]
    public string Algorithm { get; set; } = string.Empty;

    [JsonPropertyName("signatureAlgorithm")]
    public string? SignatureAlgorithm { get; set; }

    [JsonPropertyName("sigFormat")]
    public string SigFormat { get; set; } = "DER";

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("signedAt")]
    [JsonConverter(typeof(FlexibleDateTimeOffsetConverter))]
    public DateTimeOffset SignedAt { get; set; }
}

public sealed class PackagePublicKey
{
    [JsonPropertyName("owner")]
    public string Owner { get; set; } = string.Empty;

    [JsonPropertyName("spki")]
    public string Spki { get; set; } = string.Empty;

    [JsonPropertyName("fingerprint")]
    public string Fingerprint { get; set; } = string.Empty;

    [JsonPropertyName("validFrom")]
    [JsonConverter(typeof(FlexibleDateTimeOffsetConverter))]
    public DateTimeOffset ValidFrom { get; set; }

    [JsonPropertyName("validUntil")]
    [JsonConverter(typeof(FlexibleDateTimeOffsetConverter))]
    public DateTimeOffset ValidUntil { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "active";
}
