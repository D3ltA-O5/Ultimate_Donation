using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using System;
using System.Linq;
using UltimateDonation;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DonatorAdminCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly Config _config;

    public string Command => "donator";
    public string[] Aliases => new[] { "donadm", "dadm" };
    public string Description => "Admin command to manage donor roles (addrole, removerole, freeze, info, etc.).";

    public DonatorAdminCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _config = plugin.Config;
        }
    }

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!(sender is CommandSender))
        {
            response = "You must run this command in RA console.";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = GetHelpText();
            return false;
        }

        var subCommand = arguments.At(0).ToLowerInvariant();
        switch (subCommand)
        {
            case "addrole":
                {
                    if (!sender.CheckPermission("donator.addrole"))
                    {
                        response = "You don't have permission to add donor roles!";
                        return false;
                    }

                    if (arguments.Count < 4)
                    {
                        response = "Usage: donator addrole <SteamID64> <roleKey> <days>";
                        return false;
                    }

                    var steamId = arguments.At(1);
                    var roleKey = arguments.At(2).ToLowerInvariant();
                    if (!int.TryParse(arguments.At(3), out var days))
                    {
                        response = "Days must be an integer (number of days).";
                        return false;
                    }

                    if (!_config.DonatorRoles.ContainsKey(roleKey))
                    {
                        response = $"Role '{roleKey}' not found in config DonatorRoles.";
                        return false;
                    }

                    _roleManager.AddDonation(steamId, roleKey, days);
                    response = $"Donator role '{roleKey}' added to {steamId} for {days} days.";
                    return true;
                }

            case "removerole":
                {
                    if (!sender.CheckPermission("donator.removerole"))
                    {
                        response = "You don't have permission to remove donor roles!";
                        return false;
                    }

                    if (arguments.Count < 2)
                    {
                        response = "Usage: donator removerole <SteamID64>";
                        return false;
                    }

                    var steamId = arguments.At(1);
                    _roleManager.RemoveDonation(steamId);
                    response = $"Donator role removed from {steamId} (if it existed).";
                    return true;
                }

            case "freezeall":
                {
                    if (!sender.CheckPermission("donator.freezeall"))
                    {
                        response = "You don't have permission to freeze all donor roles!";
                        return false;
                    }

                    if (arguments.Count < 2)
                    {
                        response = "Usage: donator freezeall <true|false>";
                        return false;
                    }

                    var valStr = arguments.At(1).ToLowerInvariant();
                    bool freezeValue = valStr == "true" || valStr == "1";

                    _roleManager.AllDonationsFrozen = freezeValue;
                    response = freezeValue
                        ? "All donations have been FROZEN (global)."
                        : "All donations have been UNFROZEN (global).";
                    return true;
                }

            case "freezeplayer":
                {
                    if (!sender.CheckPermission("donator.freezeplayer"))
                    {
                        response = "You don't have permission to freeze/unfreeze a single player's donation!";
                        return false;
                    }

                    if (arguments.Count < 3)
                    {
                        response = "Usage: donator freezeplayer <SteamID64> <true|false>";
                        return false;
                    }

                    var steamId = arguments.At(1);
                    var valStr = arguments.At(2).ToLowerInvariant();
                    bool freezeValue = valStr == "true" || valStr == "1";

                    var donation = _roleManager.GetDonationInfo(steamId);
                    if (donation == null)
                    {
                        response = $"No donation found for player {steamId}.";
                        return false;
                    }

                    _roleManager.SetDonationFrozen(steamId, freezeValue);

                    response = freezeValue
                        ? $"Donation for {steamId} is now FROZEN (individually)."
                        : $"Donation for {steamId} is now UNFROZEN (individually).";
                    return true;
                }

            case "infoplayer":
                {
                    if (!sender.CheckPermission("donator.infoplayer"))
                    {
                        response = "You don't have permission to get info about someone's donation!";
                        return false;
                    }

                    if (arguments.Count < 2)
                    {
                        response = "Usage: donator infoplayer <SteamID64>";
                        return false;
                    }

                    var steamId = arguments.At(1);
                    var donation = _roleManager.GetDonationInfo(steamId);
                    if (donation == null)
                    {
                        response = $"No donation info found for {steamId}.";
                        return false;
                    }

                    var daysLeft = _roleManager.GetDaysLeft(steamId);
                    var isDonator = _roleManager.IsDonator(steamId);
                    response =
                        $"Donation info for {steamId}:\n" +
                        $"- Nickname: {donation.Nickname}\n" +
                        $"- Role: {donation.Role}\n" +
                        $"- ExpiryDate: {donation.ExpiryDate:yyyy-MM-dd}\n" +
                        $"- IsFrozen: {donation.IsFrozen}\n" +
                        $"- DaysLeft: {daysLeft}\n" +
                        $"- Currently recognized as Donator? {isDonator}";
                    return true;
                }

            case "listroleplayers":
                {
                    if (!sender.CheckPermission("donator.listroleplayers"))
                    {
                        response = "You don't have permission to list players by donor role!";
                        return false;
                    }

                    if (arguments.Count < 2)
                    {
                        response = "Usage: donator listroleplayers <roleKey>";
                        return false;
                    }

                    var roleKey = arguments.At(1).ToLowerInvariant();
                    var list = _roleManager.GetDonationsByRole(roleKey);
                    if (list.Count == 0)
                    {
                        response = $"No players have the role '{roleKey}'.";
                        return true;
                    }

                    var result = list
                        .Select(d => $"{d.SteamId} (Frozen={d.IsFrozen}, Expiry={d.ExpiryDate:yyyy-MM-dd})")
                        .ToList();

                    response = $"Players with role '{roleKey}' ({list.Count}):\n" + string.Join("\n", result);
                    return true;
                }

            case "listalldonations":
                {
                    if (!sender.CheckPermission("donator.listalldonations"))
                    {
                        response = "You don't have permission to list all donations!";
                        return false;
                    }

                    var all = _roleManager.GetAllDonations();
                    if (all.Count == 0)
                    {
                        response = "No donations found on server.";
                        return true;
                    }

                    var lines = all
                        .Select(d => $"{d.SteamId} | Role={d.Role} | Expires={d.ExpiryDate:yyyy-MM-dd} | Frozen={d.IsFrozen}")
                        .ToList();

                    response = $"All donations on server ({all.Count}):\n" + string.Join("\n", lines);
                    return true;
                }

            default:
                {
                    response = GetHelpText();
                    return false;
                }
        }
    }

    private string GetHelpText()
    {
        return
            "Donator Admin Command Usage:\n" +
            "  donator addrole <SteamID64> <roleKey> <days>\n" +
            "  donator removerole <SteamID64>\n" +
            "  donator freezeall <true|false>\n" +
            "  donator freezeplayer <SteamID64> <true|false>\n" +
            "  donator infoplayer <SteamID64>\n" +
            "  donator listroleplayers <roleKey>\n" +
            "  donator listalldonations\n" +
            "Example:\n" +
            "  donator addrole 76561199000000001 keter 30\n" +
            "  donator freezeall true\n" +
            "  donator listalldonations\n";
    }
}
