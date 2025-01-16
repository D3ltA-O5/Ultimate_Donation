using System;
using System.IO;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using YamlDotNet.Serialization;
using UltimateDonation;

public class DonatorPlugin : Plugin<Config>
{
    public static DonatorPlugin Instance { get; private set; }

    private RoleManager _roleManager;
    private CooldownManager _cooldownManager;
    private TranslationData _translationData; // optional

    public override string Name => "UltimateDonation";
    public override string Author => "YourName/Team";
    public override Version Version => new Version(10, 0, 0);
    public override Version RequiredExiledVersion => new Version(7, 0, 0);

    public RoleManager RoleManager => _roleManager;
    public CooldownManager CooldownManager => _cooldownManager;
    public TranslationData TranslationData => _translationData;

    public override void OnEnabled()
    {
        base.OnEnabled();
        Instance = this;

        Log.Info("[UltimateDonation] Plugin enabling...");

        if (!Config.IsEnabled)
        {
            Log.Info("[UltimateDonation] Disabled in config, aborting load.");
            return;
        }

        // create or load default config, translations
        CreateOrLoadDefaultConfig();
        CreateOrLoadTranslationsFile();

        // init
        _roleManager = new RoleManager(Config, this);
        _cooldownManager = new CooldownManager(Config, this);

        // subscribe events
        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        Log.Info("[UltimateDonation] Enabled successfully!");
    }

    public override void OnDisabled()
    {
        if (Instance == this)
        {
            Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
        }
        Instance = null;
        base.OnDisabled();
    }

    private void OnPlayerVerified(VerifiedEventArgs ev)
    {
        if (Config.Debug)
            Log.Debug($"[UltimateDonation] OnPlayerVerified => {ev.Player.UserId}");

        var sid = DonatorUtils.CleanSteamId(ev.Player.UserId);
        if (_roleManager.IsDonator(sid))
        {
            var roleK = _roleManager.GetDonatorRole(sid);
            if (Config.DonatorRoles.TryGetValue(roleK, out var dRol))
            {
                ev.Player.RankName = dRol.RankName;
                ev.Player.RankColor = dRol.RankColor;
            }
        }
    }

    private void OnRoundEnded(RoundEndedEventArgs ev)
    {
        if (Config.Debug)
            Log.Debug("[UltimateDonation] Round ended -> Reset cooldown usage counts.");
        _cooldownManager.ResetCooldowns();
    }

    public void LogDebug(string msg)
    {
        if (Config.Debug) Log.Debug(msg);
    }

    private void CreateOrLoadDefaultConfig()
    {
        string path = System.IO.Path.Combine(Paths.Configs, "UltimateDonation.yaml");
        if (!File.Exists(path))
        {
            // For simplicity, we just write a large YAML text. 
            // We'll place the 'GlobalCommandLimits' block here so that there's no duplication in each DonatorRole.
            var defaultYaml = @"is_enabled: true
debug: false

donator_roles:
  safe:
    name: ""Safe""
    badge_color: ""green""
    permissions:
      - ""changerole""
      - ""giveitem""
    rank_name: ""SAFE""
    rank_color: ""green""
    customprefixenabled: false

  euclid:
    name: ""Euclid""
    badge_color: ""orange""
    permissions:
      - ""changerole""
      - ""giveitem""
    rank_name: ""EUCLID""
    rank_color: ""orange""
    customprefixenabled: false

  keter:
    name: ""Keter""
    badge_color: ""red""
    permissions:
      - ""changerole""
      - ""giveitem""
    rank_name: ""KETER""
    rank_color: ""red""
    customprefixenabled: true

player_donations:
  - nickname: ""DonorOne""
    steam_id: ""76561199000000001""
    role: ""safe""
    expiry_date: ""2030-12-31T00:00:00""
  - nickname: ""DonorTwo""
    steam_id: ""76561199000000002""
    role: ""safe""
    expiry_date: ""2030-12-31T00:00:00""
  - nickname: ""DonorThree""
    steam_id: ""76561199000000003""
    role: ""euclid""
    expiry_date: ""2030-12-31T00:00:00""
  - nickname: ""DonorFour""
    steam_id: ""76561199000000004""
    role: ""euclid""
    expiry_date: ""2030-12-31T00:00:00""
  - nickname: ""DonorFive""
    steam_id: ""76561199000000005""
    role: ""keter""
    expiry_date: ""2030-12-31T00:00:00""
  - nickname: ""DonorSix""
    steam_id: ""76561199000000006""
    role: ""keter""
    expiry_date: ""2030-12-31T00:00:00""

blacklisted_roles:
  - ""scp3114""

blacklisted_items:
  - ""MicroHID""

scp_change_time_limit: 120
custom_prefix_global_enable: false

global_command_limits:
  safe:
    changerole: 2
    giveitem: 2
  euclid:
    changerole: 3
    giveitem: 4
  keter:
    changerole: 5
    giveitem: 5

role_aliases:
  ""173"": ""Scp173""
  ""statue"": ""Scp173""
  ""096"": ""Scp096""
  ""shyguy"": ""Scp096""
  ""079"": ""Scp079""
  ""computer"": ""Scp079""
  ""106"": ""Scp106""
  ""larry"": ""Scp106""
  ""049"": ""Scp049""
  ""doctor"": ""Scp049""
  ""0492"": ""Scp0492""
  ""zombie"": ""Scp0492""
  ""939"": ""Scp939""
  ""dog"": ""Scp939""
  ""dclass"": ""ClassD""
  ""d-boy"": ""ClassD""
  ""scientist"": ""Scientist""
  ""facilityguard"": ""FacilityGuard""
  ""guard"": ""FacilityGuard""
  ""ntf"": ""MTFPrivate""
  ""mtf"": ""MTFPrivate""
  ""chaos"": ""ChaosConscript""
  ""ci"": ""ChaosConscript""
  ""tutorial"": ""Tutorial""
  ""spectator"": ""Spectator""

item_aliases:
  ""cardjanitor"": ""KeycardJanitor""
  ""cardscientist"": ""KeycardScientist""
  ""keycard"": ""KeycardScientist""
  ""cardguard"": ""KeycardGuard""
  ""cardmtf"": ""KeycardNTFOfficer""
  ""ntfofficer"": ""KeycardNTFOfficer""
  ""cardcommander"": ""KeycardNTFCommander""
  ""admincard"": ""KeycardFacilityManager""
  ""pistol"": ""GunCOM15""
  ""com15"": ""GunCOM15""
  ""e11"": ""GunE11SR""
  ""rifle"": ""GunE11SR""
  ""shotgun"": ""GunShotgun""
  ""pumpgun"": ""GunShotgun""
  ""mp7"": ""GunMP7""
  ""smg"": ""GunMP7""
  ""ak"": ""GunAK""
  ""logicer"": ""GunLogicer""
  ""flash"": ""GrenadeFlash""
  ""hegrenade"": ""GrenadeHE""
  ""vest"": ""ArmorCombat""
  ""heavyvest"": ""ArmorHeavy""
  ""hazmat"": ""ArmorHazmat""
  ""medkit"": ""Medkit""
  ""painkillers"": ""Painkillers""
  ""adrenaline"": ""Adrenaline""
  ""scp500"": ""SCP500""
  ""scp207"": ""SCP207""
  ""radio"": ""Radio""
  ""micro"": ""MicroHID""
  ""disarmer"": ""Disarmer""
  ""camera"": ""WeaponManagerTablet""

forbidden_prefix_substrings:
  - ""admin""
  - ""administrator""
  - ""moderator""
  - ""fuck""
  - ""shit""
  - ""nazi""
  - ""owner""
";
            File.WriteAllText(path, defaultYaml);
            Log.Info($"[UltimateDonation] Created default config file: {path}");
        }
        else
        {
            Log.Info($"[UltimateDonation] Found existing config file: {path}");
        }
    }

    private void CreateOrLoadTranslationsFile()
    {
        try
        {
            var path = Path.Combine(Paths.Configs, "donat_translations.yml");
            if (!File.Exists(path))
            {
                var defText = @"
# donat_translations.yml
# English translations for UltimateDonation plugin.

help_donator_command: >
  Donator plugin help (English).
  Subcommands:
    addrole <SteamID> <RoleName> <Days> - Assign a donor role
    removerole <SteamID> - Remove donor role
    checklimits <SteamID> - Check usage limits
    listroles - Shows available donor roles
    listitems - Shows item aliases
    prefix <SteamID> <prefix> <color> - Set custom prefix
    help - Show this help

help_changerole_usage: ""Usage: .changerole <RoleAlias or ID>. E.g. .changerole 173""
help_changerole_not_donor: ""You are not a donor.""
help_changerole_round_not_started: ""The round hasn't started yet.""
help_changerole_no_perm: ""You don't have permission to change roles.""
help_changerole_limit: ""You reached your changerole limit this round.""
help_changerole_blacklisted: ""That role is blacklisted.""
help_changerole_scp_timed_out: ""It's too late to become SCP.""
help_changerole_scp_already_exists: ""An SCP of that type already exists.""

help_giveitem_usage: ""Usage: .giveitem <alias or itemtype>. E.g. .giveitem rifle""
help_giveitem_round_not_started: ""Can't give items before the round starts.""
help_giveitem_not_donor: ""You are not a donor.""
help_giveitem_no_perm: ""You have no 'giveitem' permission.""
help_giveitem_limit: ""You reached your giveitem limit.""
help_giveitem_blacklisted: ""That item is blacklisted.""

help_prefix_usage: ""Usage: donator prefix <SteamID> <Prefix> <Color>""

aliases_note: ""You can define role & item aliases in UltimateDonation.yaml""
";
                File.WriteAllText(path, defText);
                Log.Info($"[UltimateDonation] Created default translation file: {path}");
            }
            else
            {
                Log.Info($"[UltimateDonation] Found existing translation file: {path}");
            }

            var yaml = File.ReadAllText(path);
            var ds = new DeserializerBuilder().Build();
            _translationData = ds.Deserialize<TranslationData>(yaml) ?? new TranslationData();
        }
        catch (Exception ex)
        {
            Log.Error($"[UltimateDonation] Failed to load translations: {ex}");
            _translationData = new TranslationData();
        }
    }
}
