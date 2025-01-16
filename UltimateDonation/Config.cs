using System;
using System.Collections.Generic;
using Exiled.API.Interfaces;

/// <summary>
/// Developer/Editor note:
/// This config holds:
/// 1) DonatorRoles: a dictionary of roles (name, color, permissions).
/// 2) A list of PlayerDonations (each has a Nickname, SteamId, assigned role, expiry).
/// 3) Blacklists for roles/items, a limit for scp-change-time, etc.
/// 4) GlobalCommandLimits: no duplication inside each role. 
///    Instead, we store in one place: for each roleKey => dictionary of commands => limit.  
/// 5) Extended role/item aliases to help players who don't know IDs.
/// 6) ForbiddenPrefixSubstrings: words not allowed in custom prefixes.
/// 
/// The code checks userId => isDonator => role => globalCommandLimits[thatRole].
/// </summary>
public class Config : IConfig
{
    /// <summary>
    /// If false, plugin won't run.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// If true, plugin will log debug info.
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// A dictionary describing each donor role. 
    /// We do NOT store command limits here, only Permissions (which commands are allowed).
    /// For actual usage-limits, see GlobalCommandLimits below.
    /// </summary>
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

    /// <summary>
    /// A list of donor players. We store Nickname (for readability), SteamId, assigned Role, and ExpiryDate.
    /// The code will search by SteamId for checks.
    /// </summary>
    public List<PlayerDonation> PlayerDonations { get; set; } = new List<PlayerDonation>
    {
        new PlayerDonation
        {
            Nickname = "DonorOne",
            SteamId = "76561199000000001",
            Role = "safe",
            ExpiryDate = new DateTime(2030,12,31)
        },
        new PlayerDonation
        {
            Nickname = "DonorTwo",
            SteamId = "76561199000000002",
            Role = "safe",
            ExpiryDate = new DateTime(2030,12,31)
        },
        new PlayerDonation
        {
            Nickname = "DonorThree",
            SteamId = "76561199000000003",
            Role = "euclid",
            ExpiryDate = new DateTime(2030,12,31)
        },
        new PlayerDonation
        {
            Nickname = "DonorFour",
            SteamId = "76561199000000004",
            Role = "euclid",
            ExpiryDate = new DateTime(2030,12,31)
        },
        new PlayerDonation
        {
            Nickname = "DonorFive",
            SteamId = "76561199000000005",
            Role = "keter",
            ExpiryDate = new DateTime(2030,12,31)
        },
        new PlayerDonation
        {
            Nickname = "DonorSix",
            SteamId = "76561199000000006",
            Role = "keter",
            ExpiryDate = new DateTime(2030,12,31)
        },
    };

    /// <summary>
    /// Blacklisted roles (e.g. scp3114) that donors cannot become.
    /// </summary>
    public List<string> BlacklistedRoles { get; set; } = new List<string> { "scp3114" };

    /// <summary>
    /// Blacklisted items (e.g. MicroHID) that donors cannot give themselves.
    /// </summary>
    public List<string> BlacklistedItems { get; set; } = new List<string> { "MicroHID" };

    /// <summary>
    /// Number of seconds from round start after which it's too late to become an SCP.
    /// </summary>
    public float ScpChangeTimeLimit { get; set; } = 120f;

    /// <summary>
    /// If true, roles with CustomPrefixEnabled can set a custom prefix.
    /// </summary>
    public bool CustomPrefixGlobalEnable { get; set; } = false;

    /// <summary>
    /// Global dictionary: { roleKey => { command => limit } }.
    /// We don't store them in each role to avoid duplication.
    /// Example:
    ///   "safe": {"changerole":2, "giveitem":2}
    ///   "euclid": {"changerole":3, "giveitem":4}
    ///   "keter": {"changerole":5, "giveitem":5}
    /// 
    /// Developer note:
    /// If you want a single universal limit for all roles, you can do something else. But here we keep 
    /// a per-role dictionary, so we don't replicate it in DonatorRole object itself.
    /// </summary>
    public Dictionary<string, Dictionary<string, int>> GlobalCommandLimits { get; set; } = new Dictionary<string, Dictionary<string, int>>
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

    /// <summary>
    /// Aliases for roles (all possible). This helps players who don't know exact RoleTypeId.
    /// Developer note:
    /// If you want to be absolutely exhaustive, you can list every single RoleTypeId from SCPSL. 
    /// We'll provide a big set below.
    /// </summary>
    public Dictionary<string, string> RoleAliases { get; set; } = new Dictionary<string, string>
    {
        // SCP examples
        { "173", "Scp173" }, {"statue","Scp173" },
        { "096", "Scp096" }, {"shyguy","Scp096" },
        { "079", "Scp079" }, {"computer","Scp079" },
        { "106", "Scp106" }, {"larry","Scp106" },
        { "049", "Scp049" }, {"doctor","Scp049" },
        { "0492", "Scp0492" }, {"zombie","Scp0492" },
        { "939", "Scp939" }, {"dog","Scp939" },
        // Humans
        { "dclass", "ClassD" }, {"d-boy","ClassD" },
        { "scientist","Scientist" },
        { "facilityguard","FacilityGuard" },{"guard","FacilityGuard" },
        { "cadet","FacilityGuard" }, // optional
        { "ntf","MTFPrivate" }, // or "MTFSergeant", etc. depends on your version
        { "mtf","MTFPrivate" },
        { "chaos","ChaosConscript" }, {"ci","ChaosConscript" },
        { "tutorial","Tutorial" },
        { "spectator","Spectator" },
    };

    /// <summary>
    /// Aliases for items (all possible from vanilla). 
    /// Developer note:
    /// We'll try to cover all ItemTypes from SCPSL standard. 
    /// </summary>
    public Dictionary<string, string> ItemAliases { get; set; } = new Dictionary<string, string>
    {
        // Keycards
        { "cardjanitor", "KeycardJanitor" },
        { "cardscientist", "KeycardScientist" },
        { "keycard", "KeycardScientist" },
        { "cardguard", "KeycardGuard" },
        { "cardmtf", "KeycardNTFOfficer" },
        { "ntfofficer","KeycardNTFOfficer"},
        { "cardcommander","KeycardNTFCommander" },
        { "admincard","KeycardFacilityManager" }, // for example
        // Weapons
        { "pistol", "GunCOM15" },
        { "com15", "GunCOM15" },
        { "e11", "GunE11SR" },
        { "rifle", "GunE11SR" },
        { "shotgun", "GunShotgun" },
        { "pumpgun", "GunShotgun" },
        { "mp7", "GunMP7" },
        { "smg", "GunMP7" },
        { "ak", "GunAK" },
        { "logicer","GunLogicer" },
        // Grenades
        { "flash", "GrenadeFlash" },
        { "hegrenade", "GrenadeHE" },
        // Armor
        { "vest", "ArmorCombat" },
        { "heavyvest", "ArmorHeavy" },
        { "hazmat","ArmorHazmat" },
        // Medical
        { "medkit","Medkit" },
        { "painkillers","Painkillers" },
        { "adrenaline","Adrenaline" },
        { "scp500","SCP500" },
        { "scp207","SCP207" },
        // Devices
        { "radio","Radio" },
        { "micro","MicroHID" },
        { "disarmer","Disarmer" },
        { "camera","WeaponManagerTablet" }, // if you have that
    };

    /// <summary>
    /// Words or substrings that are forbidden in custom prefixes. 
    /// Developer note: Lowercase everything or do .ToLower() checks.
    /// </summary>
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

/// <summary>
/// DonatorRole: name, color, permissions. We do NOT store the numeric command-limits here (like we used to).
/// Instead, see GlobalCommandLimits in Config for that data.
/// </summary>
public class DonatorRole
{
    public string Name { get; set; }
    public string BadgeColor { get; set; }
    public List<string> Permissions { get; set; } = new List<string>();

    public string RankName { get; set; }
    public string RankColor { get; set; }
    public bool CustomPrefixEnabled { get; set; } = false;
}

/// <summary>
/// One player's donor record: Nickname (for config readability), SteamId, assigned role, expiry date.
/// </summary>
public class PlayerDonation
{
    public string Nickname { get; set; } // for reference in config
    public string SteamId { get; set; }
    public string Role { get; set; }
    public DateTime ExpiryDate { get; set; }
}
