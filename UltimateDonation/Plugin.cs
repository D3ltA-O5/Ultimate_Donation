using System;
using System.IO;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

public class DonatorPlugin : Plugin<Config>
{
    private PermissionsManager _permissionsManager;
    private RoleManager _roleManager;
    private CooldownManager _cooldownManager;
    private DiscordManager _discordManager;

    public override string Name => "UltimateDonation";
    public override string Author => "Ваш ник / или команда";
    public override Version Version => new Version(1, 0, 0);
    public override Version RequiredExiledVersion => new Version(7, 0, 0); // Пример

    public override void OnEnabled()
    {
        base.OnEnabled();

        // Инициализация
        _permissionsManager = new PermissionsManager(Path.Combine(Paths.Configs, "permissions.yml"));
        _roleManager = new RoleManager(Config);
        _cooldownManager = new CooldownManager(Config);

        UpdatePermissions();

        Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
        Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;

        // Discord
        if (!string.IsNullOrEmpty(Config.DiscordBotToken) && Config.DiscordChannelId != 0)
        {
            _discordManager = new DiscordManager(Config.DiscordBotToken, Config.DiscordGuildId, Config.DiscordChannelId, _roleManager);
            _discordManager.StartAsync().GetAwaiter().GetResult();
        }
        else
        {
            Log.Warn("DiscordBotToken или DiscordChannelId не настроены. Игнорируем запуск бота.");
        }

        Log.Info("DonatorPlugin успешно включён.");
    }

    public override void OnDisabled()
    {
        Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
        Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;

        _discordManager?.StopAsync().GetAwaiter().GetResult();
        _discordManager = null;

        base.OnDisabled();
    }

    private void OnPlayerVerified(VerifiedEventArgs ev)
    {
        // Если игрок — донатёр, выставим ему rankName и rankColor
        if (_roleManager.IsDonator(ev.Player.UserId))
        {
            var role = _roleManager.GetDonatorRole(ev.Player.UserId);
            if (Config.DonatorRoles.TryGetValue(role, out var donatorRole))
            {
                ev.Player.RankName = donatorRole.RankName;
                ev.Player.RankColor = donatorRole.RankColor;
            }
        }
    }

    private void OnRoundEnded(RoundEndedEventArgs ev)
    {
        // Сбрасываем лимиты в конце раунда (или в начале следующего, на ваш выбор)
        _cooldownManager.ResetCooldowns();
    }

    private void UpdatePermissions()
    {
        // Записываем обновлённые роли и их permissions в permissions.yml
        _permissionsManager.UpdateRolesAndPermissions(Config.DonatorRoles);
        // Обновляем список игроков
        _permissionsManager.UpdateMembers(Config.PlayerDonations);
    }
}
