using System;
using System.IO;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

/// <summary>
/// Главный класс плагина, наследуется от Plugin<Config>.
/// </summary>
public class DonatorPlugin : Plugin<Config>
{
    public static DonatorPlugin Instance { get; private set; }

    private PermissionsManager _permissionsManager;
    private RoleManager _roleManager;
    private CooldownManager _cooldownManager;

    // Название плагина (как будет отображаться в логах EXILED).
    public override string Name => "UltimateDonation";
    public override string Author => "YourName / Team";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredExiledVersion => new Version(7, 0, 0);

    // Для удобства
    public PermissionsManager PermissionsManager => _permissionsManager;
    public RoleManager RoleManager => _roleManager;
    public CooldownManager CooldownManager => _cooldownManager;

    public override void OnEnabled()
    {
        base.OnEnabled();
        Instance = this;

        Log.Info($"[UltimateDonation] Plugin is enabling. Debug = {Config.UltimateDonation.debug}");

        // Если ultimate_donation.is_enabled = false, можно сразу ничего не делать:
        if (!Config.UltimateDonation.is_enabled)
        {
            Log.Info("[UltimateDonation] Plugin is disabled via config (ultimate_donation.is_enabled = false).");
            return;
        }

        // Инициализируем менеджеры
        _permissionsManager = new PermissionsManager(Path.Combine(Paths.Configs, "permissions.yml"), this);
        _roleManager = new RoleManager(Config, this);
        _cooldownManager = new CooldownManager(Config, this);

        UpdatePermissions();

        // Подписки на события
        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        Log.Info("[UltimateDonation] Plugin has been enabled successfully!");
    }

    public override void OnDisabled()
    {
        // Если в OnEnabled ничего не инициализировалось (потому что is_enabled=false), то аккуратнее.
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

        // Проверяем донат
        if (_roleManager != null && _roleManager.IsDonator(ev.Player.UserId))
        {
            var roleKey = _roleManager.GetDonatorRole(ev.Player.UserId);
            // Пытаемся найти описание роли (DonatorRole) в config
            if (Config.UltimateDonation.donator_roles.TryGetValue(roleKey, out var donatorRole))
            {
                // Устанавливаем RankName/RankColor
                ev.Player.RankName = donatorRole.rank_name;
                ev.Player.RankColor = donatorRole.rank_color;

                LogDebug($"[UltimateDonation] Player {ev.Player.UserId} received donor rank '{donatorRole.rank_name}' ({donatorRole.rank_color}).");
            }
            else
            {
                LogDebug($"[UltimateDonation] Player {ev.Player.UserId} has donor role '{roleKey}', but it's not found in donator_roles.");
            }
        }
        else
        {
            LogDebug($"[UltimateDonation] Player {ev.Player.UserId} is not a donor.");
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
