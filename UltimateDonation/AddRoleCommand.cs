using CommandSystem;
using System;
using RemoteAdmin;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class AddRoleCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly Config _config;

    public AddRoleCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _config = plugin.Config;
        }
    }

    public string Command => "addrole";
    public string[] Aliases => new[] { "addr" };
    public string Description => "Assign a donor role to a player for N days.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        DonatorPlugin.Instance?.LogDebug($"[AddRoleCommand] Executing addrole, args={string.Join(" ", arguments)}");

        if (arguments.Count < 3)
        {
            response = "Usage: donator addrole <SteamID> <RoleName> <Days>";
            return false;
        }

        var steamId = arguments.At(0);
        var role = arguments.At(1);
        if (!int.TryParse(arguments.At(2), out var days))
        {
            response = "Days must be an integer.";
            return false;
        }

        // Проверка: есть ли такая роль
        var rolesDict = _config.UltimateDonation.donator_roles;
        if (!rolesDict.ContainsKey(role))
        {
            response = $"Role '{role}' not found in config.";
            return false;
        }

        _roleManager.AddDonation(steamId, role, days);
        response = $"Player {steamId} assigned donor role '{role}' for {days} days.";
        return true;
    }
}
