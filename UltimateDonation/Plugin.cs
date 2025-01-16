using System;
using System.IO;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using UltimateDonation;

public class DonatorPlugin : Plugin<Config>
{
    public static DonatorPlugin Instance { get; private set; }

    private PermissionsManager _permissionsManager;
    private RoleManager _roleManager;
    private CooldownManager _cooldownManager;

    public override string Name => "UltimateDonation";
    public override string Author => "YourName / Team";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredExiledVersion => new Version(7, 0, 0);

    public PermissionsManager PermissionsManager => _permissionsManager;
    public RoleManager RoleManager => _roleManager;
    public CooldownManager CooldownManager => _cooldownManager;

    public override void OnEnabled()
    {
        base.OnEnabled();
        Instance = this;

        Log.Info($"[UltimateDonation] Plugin is enabling. Debug = {Config.UltimateDonation.debug}");

        if (!Config.UltimateDonation.is_enabled)
        {
            Log.Info("[UltimateDonation] Plugin is disabled via config (ultimate_donation.is_enabled = false).");
            return;
        }

        _permissionsManager = new PermissionsManager(Path.Combine(Paths.Configs, "permissions.yml"), this);
        _roleManager = new RoleManager(Config, this);
        _cooldownManager = new CooldownManager(Config, this);

        UpdatePermissions();

        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        Log.Info("[UltimateDonation] Plugin has been enabled successfully!");
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
        if (Config.UltimateDonation.debug)
            Log.Debug($"[UltimateDonation] OnPlayerVerified: {ev.Player.UserId}");

        // Очищаем суффикс "@steam"
        var cleanedId = DonatorUtils.CleanSteamId(ev.Player.UserId);

        if (_roleManager != null && _roleManager.IsDonator(cleanedId))
        {
            var roleKey = _roleManager.GetDonatorRole(cleanedId);
            if (Config.UltimateDonation.donator_roles.TryGetValue(roleKey, out var donatorRole))
            {
                ev.Player.RankName = donatorRole.rank_name;
                ev.Player.RankColor = donatorRole.rank_color;

                LogDebug($"[UltimateDonation] Player {cleanedId} got donor rank '{donatorRole.rank_name}' ({donatorRole.rank_color}).");
            }
            else
            {
                LogDebug($"[UltimateDonation] Player {cleanedId} has donor role '{roleKey}', but it's not found in donator_roles.");
            }
        }
        else
        {
            LogDebug($"[UltimateDonation] Player {cleanedId} is not a donor.");
        }
    }

    private void OnRoundEnded(RoundEndedEventArgs ev)
    {
        LogDebug("[UltimateDonation] Round ended, resetting cooldown usage counters.");
        _cooldownManager?.ResetCooldowns();
    }

    private void UpdatePermissions()
    {
        LogDebug("[UltimateDonation] Updating PermissionsManager with roles and members...");
        _permissionsManager?.UpdateRolesAndPermissions(Config.UltimateDonation.donator_roles);
        _permissionsManager?.UpdateMembers(Config.UltimateDonation.player_donations);
    }

    public void LogDebug(string msg)
    {
        if (Config.UltimateDonation.debug)
            Log.Debug(msg);
    }
}
