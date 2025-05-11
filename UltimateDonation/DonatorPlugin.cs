using System;
using System.IO;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using YamlDotNet.Serialization;
using UltimateDonation;

public class DonatorPlugin : Plugin<Config>
{
    public static DonatorPlugin Instance { get; private set; }

    private RoleManager _roleManager;
    private CooldownManager _cooldownManager;
    private Translation _translation;

    public override string Name => "UltimateDonation";
    public override string Author => "D3ltA_O5";
    public override Version Version => new Version(1, 1, 0);
    public override Version RequiredExiledVersion => new Version(7, 0, 0);
    public ITranslation Translation => _translation;

    public RoleManager RoleManager => _roleManager;
    public CooldownManager CooldownManager => _cooldownManager;

    public DonationsManager DonationsManager { get; private set; }


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

        CreateOrLoadTranslationsFile();

        DonationsManager = new DonationsManager(this);

        _roleManager = new RoleManager(Config, this);
        _cooldownManager = new CooldownManager(Config, this);

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
            var roleKey = _roleManager.GetDonatorRole(sid);
            if (Config.DonatorRoles.TryGetValue(roleKey, out var dRol))
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

    private void CreateOrLoadTranslationsFile()
    {
        try
        {
            // 1. Создаём папку Ultimate_Donation (если нет)
            var donationFolder = Path.Combine(Paths.Configs, "Ultimate_Donation");
            Directory.CreateDirectory(donationFolder);

            // 2. Наш файл переводов теперь лежит в этой папке
            var path = Path.Combine(donationFolder, "donat_translations.yml");

            if (!File.Exists(path))
            {
                var defText = @"
################################################################################
# Donator translations (single-line, single-quoted).                           #
# Используем \n в строке, чтобы при выводе текста была разметка (переносы).
################################################################################

help_donator_command: 'Donator plugin help (English). Subcommands: addrole <SteamID> <RoleName> <Days>, removerole <SteamID>, freezeall <true|false>, freezeplayer <SteamID> <true|false>, infoplayer <SteamID>, listroleplayers <RoleKey>, listalldonations.'
help_changerole_usage: 'Usage: .changerole <RoleAlias>. E.g. .changerole 173'
help_changerole_not_donor: 'You are not a donor.'
help_changerole_round_not_started: 'The round has not started yet.'
help_changerole_no_perm: 'You do not have permission to change roles.'
help_changerole_limit: 'You reached your changerole limit this round.'
help_changerole_blacklisted: 'That role is blacklisted.'
help_changerole_scp_timed_out: 'Too late to become SCP.'
help_changerole_scp_already_exists: 'That SCP already exists.'

help_giveitem_usage: 'Usage: .giveitem <alias or itemtype>. E.g. .giveitem rifle'
help_giveitem_round_not_started: 'Cannot use .giveitem before the round starts.'
help_giveitem_not_donor: 'You are not a donor.'
help_giveitem_no_perm: 'You do not have ''giveitem'' permission.'
help_giveitem_limit: 'You reached your giveitem limit.'
help_giveitem_blacklisted: 'That item is blacklisted.'

help_prefix_usage: 'Usage: donator prefix <SteamId> <Prefix> <Color>'
aliases_note: 'You can define role & item aliases in this file below.'
only_player_can_use_command: 'Only a player can use this command (not console).'
player_object_not_found: 'Failed to retrieve your player object. Please try again.'
missing_donor_role_in_config: 'Your donor role is missing in config.'

unknown_role_alias: 'Unknown role alias/id'
change_role_success: 'You changed your role to {roleName}.'
cannot_give_item_as_scp: 'You cannot give items while you are an SCP.'
unknown_item_alias: 'Unknown item alias'
give_item_success: 'You received {itemType}.'

mydon_only_player: 'This command can only be used by players in the client console.'
mydon_not_donor: 'You are not a donor, or your donation has expired.'
mydon_role_not_configured: 'Your donor role is not configured correctly. Please contact an administrator.'
mydon_no_limits_found: 'No global command limits found for your role ''{roleKey}''.'
mydon_no_commands_tracked: 'No commands available for usage tracking.'

mydon_status_info: |
  === Your Donation Status ===
  - Role: {roleName} (Key: {roleKey})
  - Days Left: {daysLeft}
  - Permissions: {permissions}
  - Command Usage This Round: {usageSummary}
  (Tip: Use '.changerole' or '.giveitem' if allowed by your role.)

donator_only_players: 'This command can only be used by a player in client console.'
prefix_not_allowed: 'This donor role does not allow custom prefixes.'
prefix_set_success: 'Custom prefix ''{prefixValue}'' color=''{colorValue}'' set successfully!'

################################################################################
# Single-line role_aliases and item_aliases
################################################################################

role_aliases:
  '173': 'Scp173'
  'statue': 'Scp173'
  '096': 'Scp096'
  'shyguy': 'Scp096'
  '079': 'Scp079'
  'computer': 'Scp079'
  '106': 'Scp106'
  'larry': 'Scp106'
  '049': 'Scp049'
  'doctor': 'Scp049'
  '0492': 'Scp0492'
  'zombie': 'Scp0492'
  '939': 'Scp939'
  'dog': 'Scp939'
  'dclass': 'ClassD'
  'd-boy': 'ClassD'
  'scientist': 'Scientist'
  'facilityguard': 'FacilityGuard'
  'guard': 'FacilityGuard'
  'ntf': 'MTFPrivate'
  'mtf': 'MTFPrivate'
  'chaos': 'ChaosConscript'
  'ci': 'ChaosConscript'
  'tutorial': 'Tutorial'
  'spectator': 'Spectator'

item_aliases:
  'cardjanitor': 'KeycardJanitor'
  'cardscientist': 'KeycardScientist'
  'keycard': 'KeycardScientist'
  'cardguard': 'KeycardGuard'
  'cardcommander': 'KeycardNTFCommander'
  'admincard': 'KeycardFacilityManager'
  'pistol': 'GunCOM15'
  'com15': 'GunCOM15'
  'e11': 'GunE11SR'
  'rifle': 'GunE11SR'
  'shotgun': 'GunShotgun'
  'pumpgun': 'GunShotgun'
  'mp7': 'GunMP7'
  'smg': 'GunMP7'
  'ak': 'GunAK'
  'logicer': 'GunLogicer'
  'flash': 'GrenadeFlash'
  'hegrenade': 'GrenadeHE'
  'vest': 'ArmorCombat'
  'heavyvest': 'ArmorHeavy'
  'hazmat': 'ArmorHazmat'
  'medkit': 'Medkit'
  'painkillers': 'Painkillers'
  'adrenaline': 'Adrenaline'
  'scp500': 'SCP500'
  'scp207': 'SCP207'
  'radio': 'Radio'
  'micro': 'MicroHID'
";
                File.WriteAllText(path, defText);
                Log.Info($"[UltimateDonation] Created default single-line translation file: {path}");
            }

            var yaml = File.ReadAllText(path);
            var ds = new DeserializerBuilder().Build();
            _translation = ds.Deserialize<Translation>(yaml) ?? new Translation();
        }
        catch (Exception ex)
        {
            Log.Error($"[UltimateDonation] Failed to load translations: {ex}");
            _translation = new Translation();
        }
    }
}
