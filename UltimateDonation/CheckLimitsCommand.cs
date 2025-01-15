using CommandSystem;
using PluginAPI.Roles;
using System;

public class CheckLimitsCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public CheckLimitsCommand(RoleManager roleManager, CooldownManager cooldownManager, Config config)
    {
        _roleManager = roleManager;
        _cooldownManager = cooldownManager;
        _config = config;
    }

    public string Command => "checklimits";

    public string[] Aliases => new[] { "cl" };

    public string Description => "Проверить лимиты донатера.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count < 1)
        {
            response = "Использование: donator checklimits <SteamID>";
            return false;
        }

        var targetId = arguments.At(0);
        if (!_roleManager.IsDonator(targetId))
        {
            response = "Игрок не имеет донатной роли.";
            return false;
        }

        var role = _roleManager.GetDonatorRole(targetId);
        var roleLimits = _config.DonatorRoles[role];
        response = $"Лимиты для игрока {targetId}: {roleLimits}";
        return true;
    }
}
