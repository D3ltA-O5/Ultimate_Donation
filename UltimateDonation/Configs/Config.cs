using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;

    public Dictionary<string, DonatorRole> DonatorRoles { get; set; } = new Dictionary<string, DonatorRole>
    {
        {
            "safe",
            new DonatorRole
            {
                Name = "Safe",
                BadgeColor = "green",
                Permissions = new List<string> { "changerole", "giveitem" },
                RankName = "SAFE",
                RankColor = "green",
                CustomPrefixEnabled = false
            }
        },
        {
            "euclid",
            new DonatorRole
            {
                Name = "Euclid",
                BadgeColor = "orange",
                Permissions = new List<string> { "changerole", "giveitem" },
                RankName = "EUCLID",
                RankColor = "orange",
                CustomPrefixEnabled = false
            }
        },
        {
            "keter",
            new DonatorRole
            {
                Name = "Keter",
                BadgeColor = "red",
                Permissions = new List<string> { "changerole", "giveitem" },
                RankName = "KETER",
                RankColor = "red",
                CustomPrefixEnabled = true
            }
        }
    };

    public List<string> BlacklistedRoles { get; set; } = new List<string> { "scp3114" };
    public List<string> BlacklistedItems { get; set; } = new List<string> { "MicroHID" };
    public float ScpChangeTimeLimit { get; set; } = 120f;
    public bool CustomPrefixGlobalEnable { get; set; } = false;

    public Dictionary<string, Dictionary<string, int>> GlobalCommandLimits { get; set; }
        = new Dictionary<string, Dictionary<string, int>>
    {
        {
            "safe", new Dictionary<string,int>
            {
                { "changerole", 2 },
                { "giveitem", 2 }
            }
        },
        {
            "euclid", new Dictionary<string,int>
            {
                { "changerole", 3 },
                { "giveitem", 4 }
            }
        },
        {
            "keter", new Dictionary<string,int>
            {
                { "changerole", 5 },
                { "giveitem", 5 }
            }
        }
    };

    public List<string> ForbiddenPrefixSubstrings { get; set; } = new List<string>
    {
        "admin",
        "administrator",
        "moderator",
        "fuck",
        "shit",
        "nazi",
        "owner"
    };
}

public class DonatorRole
{
    public string Name { get; set; }
    public string BadgeColor { get; set; }
    public List<string> Permissions { get; set; } = new List<string>();
    public string RankName { get; set; }
    public string RankColor { get; set; }
    public bool CustomPrefixEnabled { get; set; } = false;
}

public class PlayerDonation
{
    public string Nickname { get; set; }
    public string SteamId { get; set; }
    public string Role { get; set; }
    public DateTime ExpiryDate { get; set; }
    public bool IsFrozen { get; set; } = false;
    public DateTime? FreezeStartedAt { get; set; } = null;
}
