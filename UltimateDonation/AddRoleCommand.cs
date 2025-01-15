using CommandSystem;
using PluginAPI.Roles;
using System;

public class AddRoleCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public AddRoleCommand(RoleManager roleManager, CooldownManager cooldownManager, Config config)
    {
        _roleManager = roleManager;
        _cooldownManager = cooldownManager;
        _config = config;
    }

    public string Command => "addrole";

    public string[] Aliases => new[] { "addr" };

    public string Description => "Добавить роль донатера.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count < 3)
        {
            response = "Использование: donator addrole <SteamID> <RoleName> <Days>";
            return false;
        }

        var targetId = arguments.At(0);
        var roleName = arguments.At(1);
        if (!int.TryParse(arguments.At(2), out var days))
        {
            response = "Количество дней должно быть числом.";
            return false;
        }

        if (!_config.DonatorRoles.ContainsKey(roleName))
        {
            response = $"Роль {roleName} не существует.";
            return false;
        }

        _roleManager.AddDonation(targetId, roleName, days);
        response = $"Роль {roleName} успешно добавлена игроку {targetId} на {days} дней.";
        return true;
    }
}
