using CommandSystem;
using RemoteAdmin;
using System;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DonatorCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public DonatorCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _cooldownManager = plugin.CooldownManager;
            _config = plugin.Config;
        }
    }

    public string Command => "donator";
    public string[] Aliases => new[] { "don" };
    public string Description => "Manage donor privileges (via RA).";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        DonatorPlugin.Instance?.LogDebug($"[DonatorCommand] Executing 'donator', args={string.Join(" ", arguments)}");

        if (arguments.Count == 0)
        {
            response = "Usage: donator <addrole|removerole|checklimits> ...";
            return false;
        }

        var subCmd = arguments.At(0).ToLowerInvariant();
        switch (subCmd)
        {
            case "addrole":
                if (arguments.Count < 4)
                {
                    response = "Usage: donator addrole <SteamID> <RoleName> <Days>";
                    return false;
                }
                {
                    var steamId = arguments.At(1);
                    var roleName = arguments.At(2);
                    if (!int.TryParse(arguments.At(3), out var days))
                    {
                        response = "Days must be an integer.";
                        return false;
                    }
                    if (!_config.UltimateDonation.donator_roles.ContainsKey(roleName))
                    {
                        response = $"Role {roleName} not found in config.";
                        return false;
                    }
                    _roleManager.AddDonation(steamId, roleName, days);
                    response = $"Player {steamId} was given donor role {roleName} for {days} days.";
                    return true;
                }

            case "removerole":
                if (arguments.Count < 2)
                {
                    response = "Usage: donator removerole <SteamID>";
                    return false;
                }
                {
                    var steamId = arguments.At(1);
                    _roleManager.RemoveDonation(steamId);
                    response = $"Donor role removed from {steamId}.";
                    return true;
                }

            case "checklimits":
                if (arguments.Count < 2)
                {
                    response = "Usage: donator checklimits <SteamID>";
                    return false;
                }
                {
                    var steamId = arguments.At(1);
                    if (!_roleManager.IsDonator(steamId))
                    {
                        response = "Player is not an active donor.";
                        return false;
                    }
                    var roleKey = _roleManager.GetDonatorRole(steamId);
                    if (!_config.UltimateDonation.donator_roles.TryGetValue(roleKey, out var roleData))
                    {
                        response = $"Could not find donor role '{roleKey}' in config.";
                        return false;
                    }

                    response = $"Donor info for player {steamId}, role='{roleKey}':\n" +
                               $"- role_change_limit: {roleData.role_change_limit}\n" +
                               $"- item_give_limit: {roleData.item_give_limit}\n" +
                               "command_limits: " + string.Join(", ", roleData.command_limits);
                    return true;
                }

            default:
                response = "Unknown subcommand. Use addrole/removerole/checklimits.";
                return false;
        }
    }
}
