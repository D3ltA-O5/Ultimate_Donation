using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;

    public string DiscordBotToken { get; set; } = string.Empty;
    public ulong DiscordChannelId { get; set; }

    public Dictionary<string, DonatorRole> DonatorRoles { get; set; } = new Dictionary<string, DonatorRole>();
    public Dictionary<string, PlayerDonation> PlayerDonations { get; set; } = new Dictionary<string, PlayerDonation>();
}

public class DonatorRole
{
    public string Name { get; set; }
    public string BadgeColor { get; set; } = "red";
    public int RoleChangeLimit { get; set; } = 3;
    public int ItemGiveLimit { get; set; } = 5;
    public List<string> Permissions { get; set; } = new List<string>();

    public string RankName { get; set; } = "Donator";
    public string RankColor { get; set; } = "red";
}

public class PlayerDonation
{
    public string SteamId { get; set; }
    public string Role { get; set; }
    public DateTime ExpiryDate { get; set; }
}
