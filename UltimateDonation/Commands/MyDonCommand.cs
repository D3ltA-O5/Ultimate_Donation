using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using System;
using System.Linq;
using UltimateDonation;

[CommandHandler(typeof(ClientCommandHandler))] 
public class MyDonCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public MyDonCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _cooldownManager = plugin.CooldownManager;
            _config = plugin.Config;
        }
    }

    public string Command => "mydon";
    public string[] Aliases => new[] { "mydonation", "mydn", "md" };
    public string Description => "Check your donor status, days left, and command usage limits with multiline formatting.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var t = DonatorPlugin.Instance?.Translation as Translation;
        if (t == null)
        {
            response = "Translation not loaded.";
            return false;
        }

        if (!(sender is PlayerCommandSender playerSender))
        {
            response = t.MyDonOnlyPlayer ?? "This command can only be used by players in the client console.";
            return false;
        }

        var player = Player.Get(playerSender.ReferenceHub);
        if (player == null)
        {
            response = t.PlayerObjectNotFound ?? "Failed to retrieve your player object. Please try again.";
            return false;
        }

        var steamId = DonatorUtils.CleanSteamId(player.UserId);
        if (!_roleManager.IsDonator(steamId))
        {
            response = t.MyDonNotDonor ?? "You are not a donor, or your donation has expired.";
            return false;
        }

        var roleKey = _roleManager.GetDonatorRole(steamId);
        if (!_config.DonatorRoles.TryGetValue(roleKey, out var donorRole))
        {
            response = t.MyDonRoleNotConfigured
                ?? "Your donor role is not configured correctly. Please contact an administrator.";
            return false;
        }

        int daysLeft = _roleManager.GetDaysLeft(steamId);

        if (!_config.GlobalCommandLimits.TryGetValue(roleKey, out var commandLimits))
        {
            response = t.MyDonNoLimitsFound.Replace("{roleKey}", roleKey);
            return false;
        }

        var usageInfo = commandLimits
            .Select(limit =>
            {
                var commandName = limit.Key;  
                var maxUses = limit.Value;     
                int used = _cooldownManager.GetUsageCount(steamId, commandName);
                return $"{commandName}: {used}/{maxUses}";
            })
            .ToList();

        string usageSummary = usageInfo.Any()
            ? string.Join("; ", usageInfo)
            : t.MyDonNoCommandsTracked ?? "No commands available for usage tracking.";

        var textTemplate = t.MyDonStatusInfo;

        response = textTemplate
            .Replace("{roleName}", donorRole.Name)
            .Replace("{roleKey}", roleKey)
            .Replace("{daysLeft}", daysLeft.ToString())
            .Replace("{permissions}", string.Join(", ", donorRole.Permissions))
            .Replace("{usageSummary}", usageSummary);

        response = response.Replace("\\n", "\n");

        return true;
    }
}
