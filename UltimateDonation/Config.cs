using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;
using YamlDotNet.Serialization;

/// <summary>
/// Главный класс, который EXILED использует для чтения/записи конфигурации плагина.
/// Обратите внимание: верхний уровень - это класс Config (IConfig), а вся логика
/// ultimate_donation (is_enabled, debug и т.д.) лежит в поле UltimateDonationSection.
/// </summary>
public class Config : IConfig
{
    // Это поле нужно EXILED для включения/выключения плагина целиком.
    public bool IsEnabled { get; set; } = true;

    public bool Debug { get; set; } = true;

    // Ниже - сам «раздел» ultimate_donation в стиле exiled_events
    [YamlMember(Alias = "ultimate_donation")]
    public UltimateDonationSection UltimateDonation { get; set; } = new UltimateDonationSection();
}

/// <summary>
/// Раздел ultimate_donation: хранит is_enabled, debug, donator_roles, player_donations
/// в стиле snake_case. Так у вас YAML будет выглядеть, как в exiled_events.
/// </summary>
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
            "76561199EXAMPLEID1",
            new PlayerDonation
            {
                steam_id = "76561199EXAMPLEID1",
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
        },
    };
}

/// <summary>
/// Донат-роль, поля в нижнем регистре: name, badge_color, и т.д.
/// </summary>
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

/// <summary>
/// Инфа о донате у игрока: steam_id, роль, дата окончания.
/// </summary>
public class PlayerDonation
{
    public string steam_id { get; set; }
    public string role { get; set; }
    public DateTime expiry_date { get; set; }
}
