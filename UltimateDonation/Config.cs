using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;

    public string DiscordBotToken { get; set; } = string.Empty;
    public ulong DiscordGuildId { get; set; }
    public ulong DiscordChannelId { get; set; } = 0;

    /// <summary>
    /// Словарь донатных ролей:
    ///   ключ = имя роли,
    ///   значение = объект DonatorRole (цвет бэджа, лимиты, permissions и т.д.)
    /// </summary>
    public Dictionary<string, DonatorRole> DonatorRoles { get; set; } = new Dictionary<string, DonatorRole>();

    /// <summary>
    /// Список текущих донатов (донатеров):
    ///   ключ = SteamID,
    ///   значение = информация о донате (какая роль и до какой даты).
    /// </summary>
    public Dictionary<string, PlayerDonation> PlayerDonations { get; set; } = new Dictionary<string, PlayerDonation>();
}

public class DonatorRole
{
    public string Name { get; set; }
    public string BadgeColor { get; set; } = "red";

    // Допустим, сколько раз за раунд можно сменить себе роль
    public int RoleChangeLimit { get; set; } = 3;
    // Сколько раз можно выдать предмет
    public int ItemGiveLimit { get; set; } = 5;

    /// <summary>
    /// Permissions — это именно те команды, к которым даём доступ
    /// (например, "changerole", "giveitem", "respawn", "forceclass", и т. п.)
    /// </summary>
    public List<string> Permissions { get; set; } = new List<string>();

    /// <summary>
    /// Ограничения на использование конкретных команд. 
    /// Пример: "changerole" => 2, значит команду changerole можно использовать 2 раза за раунд.
    /// </summary>
    public Dictionary<string, int> CommandLimits { get; set; } = new Dictionary<string, int>();

    // Поле RankName и RankColor, если нужно в самой игре менять плашку (Rank)
    public string RankName { get; set; } = "Donator";
    public string RankColor { get; set; } = "red";
}

public class PlayerDonation
{
    public string SteamId { get; set; }
    public string Role { get; set; }
    public DateTime ExpiryDate { get; set; }
}
