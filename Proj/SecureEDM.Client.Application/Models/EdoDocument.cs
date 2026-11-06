namespace SecureEDM.Client.Application.Models;

/// <summary>Документ локального реестра документооборота.</summary>
public sealed class EdoDocument
{
    public string Id { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    /// <summary>SHA-256 канонизированного представления документа.</summary>
    public string Hash { get; set; } = string.Empty;

    public int Version { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public List<EdoDocumentHistory> History { get; set; } = [];
}

public sealed class EdoDocumentHistory
{
    public int Version { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset ArchivedAt { get; set; }
}
