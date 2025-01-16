using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using System;
using PlayerRoles;

[CommandHandler(typeof(ClientCommandHandler))]
public class ChangeRoleCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public ChangeRoleCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _cooldownManager = plugin.CooldownManager;
            _config = plugin.Config;
        }
    }

    public string Command => "changerole";
    public string[] Aliases => new[] { "cr" };
    public string Description => "Allows a donor to change their own role.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        DonatorPlugin.Instance?.LogDebug($"[ChangeRoleCommand] Executing changerole, args={string.Join(" ", arguments)}");

        if (!(sender is PlayerCommandSender playerSender))
        {
            response = "This command can only be used by a player.";
            return false;
        }

        var player = Player.Get(playerSender.ReferenceHub);
        if (player == null)
        {
            response = "Failed to find the player object.";
            return false;
        }

        if (!_roleManager.IsDonator(player.UserId))
        {
            response = "You are not a donor.";
            return false;
        }

        var roleName = _roleManager.GetDonatorRole(player.UserId);
        var rolesDict = _config.UltimateDonation.donator_roles;
        if (!rolesDict.TryGetValue(roleName, out var donatorRole))
        {
            response = "Your donor role is missing in config.";
            return false;
        }

        // Проверяем, есть ли команда "changerole" в permissions
        if (donatorRole.permissions == null || !donatorRole.permissions.Contains("changerole"))
        {
            response = "You do not have permission to change your role.";
            return false;
        }

        // Проверка лимита
        if (!_cooldownManager.CanExecuteCommand(player.UserId, roleName, "changerole"))
        {
            response = "You have reached the usage limit for changing roles this round.";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "Usage: .changerole <RoleTypeId or name>";
            return false;
        }

        var targetRoleArg = arguments.At(0);
        var newRole = ParseRole(targetRoleArg);
        if (newRole == RoleTypeId.None)
        {
            response = $"Unknown role: {targetRoleArg}";
            return false;
        }

        player.Role.Set(newRole);
        _cooldownManager.RegisterCommandUsage(player.UserId, roleName, "changerole");

        response = $"You changed your role to {newRole}.";
        return true;
    }

    private RoleTypeId ParseRole(string arg)
    {
        if (int.TryParse(arg, out int intRole))
        {
            if (Enum.IsDefined(typeof(RoleTypeId), intRole))
                return (RoleTypeId)intRole;
        }
        else
        {
            foreach (RoleTypeId r in Enum.GetValues(typeof(RoleTypeId)))
            {
                if (r.ToString().Equals(arg, StringComparison.OrdinalIgnoreCase))
                    return r;
            }
        }
        return RoleTypeId.None;
    }
}
