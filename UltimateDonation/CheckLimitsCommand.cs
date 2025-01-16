using CommandSystem;
using RemoteAdmin;
using System;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CheckLimitsCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public CheckLimitsCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _cooldownManager = plugin.CooldownManager;
            _config = plugin.Config;
        }
    }

    public string Command => "checklimits";
    public string[] Aliases => new[] { "cl" };
    public string Description => "Check a player's donor usage limits.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        DonatorPlugin.Instance?.LogDebug($"[CheckLimitsCommand] checklimits, args={string.Join(" ", arguments)}");

        if (arguments.Count < 1)
        {
            response = "Usage: checklimits <SteamID>";
            return false;
        }

        var steamId = arguments.At(0);
        if (!_roleManager.IsDonator(steamId))
        {
            response = "This player is not an active donor.";
            return false;
        }
        var roleKey = _roleManager.GetDonatorRole(steamId);
        var rolesDict = _config.UltimateDonation.donator_roles;
        if (!rolesDict.TryGetValue(roleKey, out var roleData))
        {
            response = $"Could not find donor role '{roleKey}' in config.";
            return false;
        }

        response = $"Player {steamId} has donor role '{roleKey}'.\n" +
                   $"- role_change_limit: {roleData.role_change_limit}\n" +
                   $"- item_give_limit: {roleData.item_give_limit}\n" +
                   $"command_limits: {string.Join(", ", roleData.command_limits)}";
        return true;
    }
}
