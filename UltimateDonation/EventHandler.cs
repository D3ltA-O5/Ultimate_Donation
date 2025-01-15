using Exiled.API.Features;
using CommandSystem;
using RemoteAdmin;
using System;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs;
using PluginAPI.Roles;

public class EventHandlers
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public EventHandlers(RoleManager roleManager, CooldownManager cooldownManager, Config config)
    {
        _roleManager = roleManager;
        _cooldownManager = cooldownManager;
        _config = config;
    }

    // Обработчик события проверки игрока
    public void OnPlayerVerified(VerifiedEventArgs ev)
    {
        if (_roleManager.IsDonator(ev.Player.UserId))
        {
            var role = _roleManager.GetDonatorRole(ev.Player.UserId);
            if (_config.DonatorRoles.TryGetValue(role, out var donatorRole))
            {
                ev.Player.RankName = donatorRole.Name;
                ev.Player.RankColor = donatorRole.BadgeColor;
            }
        }
    }
}
