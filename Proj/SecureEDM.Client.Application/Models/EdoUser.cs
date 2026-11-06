namespace SecureEDM.Client.Application.Models;

/// <summary>Пользователь учебного прототипа и его роль в документообороте.</summary>
public sealed class EdoUser(string id, string name, string role, string title)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string Role { get; } = role;
    public string Title { get; } = title;

    public string DisplayName => $"{Name} — {Title}";
}

public static class EdoUsers
{
    public static readonly IReadOnlyList<EdoUser> All =
    [
        new("u1", "Иванов И. И.", "author", "Автор"),
        new("u2", "Петрова А. С.", "verifier", "Проверяющий"),
        new("u3", "Сидоров П. П.", "admin", "Администратор"),
    ];
}
