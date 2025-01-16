using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;
using YamlDotNet.Serialization;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = true;

    [YamlMember(Alias = "ultimate_donation")]
    public UltimateDonationSection UltimateDonation { get; set; } = new UltimateDonationSection();
}

public class UltimateDonationSection
{
    [YamlMember(Alias = "is_enabled")]
    public bool is_enabled { get; set; } = true;

    [YamlMember(Alias = "debug")]
    public bool debug { get; set; } = false;

    [YamlMember(Alias = "donator_roles")]
    public Dictionary<string, DonatorRole> donator_roles { get; set; } = new Dictionary<string, DonatorRole>
    {
        {
            "vip",
            new DonatorRole
            {
                name = "VIP",
                badge_color = "green",
                role_change_limit = 2,
                item_give_limit = 2,
                permissions = new List<string> { "changerole", "giveitem" },
                command_limits = new Dictionary<string, int>
                {
                    { "changerole", 2 },
                    { "giveitem", 2 },
                },
                rank_name = "VIP",
                rank_color = "green"
            }
        },
        {
            "premium",
            new DonatorRole
            {
                name = "Premium",
                badge_color = "blue",
                role_change_limit = 5,
                item_give_limit = 5,
                permissions = new List<string> { "changerole", "giveitem", "some_other_cmd" },
                command_limits = new Dictionary<string, int>
                {
                    { "changerole", 5 },
                    { "giveitem", 5 },
                    { "some_other_cmd", 3 },
                },
                rank_name = "Premium",
                rank_color = "blue"
            }
        }
    };

    [YamlMember(Alias = "player_donations")]
    public Dictionary<string, PlayerDonation> player_donations { get; set; } = new Dictionary<string, PlayerDonation>
    {
        {
            "76561199481494871",
            new PlayerDonation
            {
                steam_id = "76561199481494871",
                role = "vip",
                expiry_date = new DateTime(2030, 12, 31, 23, 59, 59)
            }
        },
        {
            "76561199EXAMPLEID2",
            new PlayerDonation
            {
                steam_id = "76561199EXAMPLEID2",
                role = "premium",
                expiry_date = new DateTime(2030, 12, 31, 23, 59, 59)
            }
        }
    };
}

public class DonatorRole
{
    public string name { get; set; }
    public string badge_color { get; set; }
    public int role_change_limit { get; set; }
    public int item_give_limit { get; set; }
    public List<string> permissions { get; set; }
    public Dictionary<string, int> command_limits { get; set; }
    public string rank_name { get; set; }
    public string rank_color { get; set; }
}

public class PlayerDonation
{
    public string steam_id { get; set; }
    public string role { get; set; }
    public DateTime expiry_date { get; set; }
}
