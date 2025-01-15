using CommandSystem;
using Exiled.API.Features;
using Exiled.Events.Handlers;
using System.IO;

public class DonatorPlugin : Plugin<Config>
{
    private PermissionsManager _permissionsManager;
    private RoleManager _roleManager;
    private CooldownManager _cooldownManager;
    private EventHandlers _eventHandlers;
    private DiscordManager _discordManager;

    public override void OnEnabled()
    {
        var permissionsFilePath = Path.Combine(Paths.Configs, "permissions.yml");

        _permissionsManager = new PermissionsManager(permissionsFilePath);
        _roleManager = new RoleManager(Config);
        _cooldownManager = new CooldownManager();

        _eventHandlers = new EventHandlers(_roleManager, _cooldownManager, Config);

        UpdatePermissions();

        Exiled.Events.Handlers.Player.Verified += _eventHandlers.OnPlayerVerified;

        // Инициализация Discord Manager
        if (!string.IsNullOrEmpty(Config.DiscordBotToken) && Config.DiscordChannelId != 0)
        {
            _discordManager = new DiscordManager(
                Config.DiscordBotToken,
                Config.DiscordChannelId,
                _permissionsManager,
                _roleManager
            );
            _discordManager.StartAsync().GetAwaiter().GetResult();
        }
        else
        {
            Log.Warn("Токен Discord-бота или ID канала не настроены в конфигурации.");
        }

        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        // Отписка от событий
        Exiled.Events.Handlers.Player.Verified -= _eventHandlers.OnPlayerVerified;

        _discordManager?.StopAsync().GetAwaiter().GetResult();

        base.OnDisabled();
    }

    private void UpdatePermissions()
    {
        _permissionsManager.UpdateRolesAndPermissions(Config.DonatorRoles);
        _permissionsManager.UpdateMembers(Config.PlayerDonations);
    }
}
