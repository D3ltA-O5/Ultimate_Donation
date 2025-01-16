using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using System;
using System.Linq;
using InventorySystem.Items;
using UltimateDonation;

[CommandHandler(typeof(ClientCommandHandler))]
public class GiveItemCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public GiveItemCommand()
    {
        var pl = DonatorPlugin.Instance;
        if (pl != null)
        {
            _roleManager = pl.RoleManager;
            _cooldownManager = pl.CooldownManager;
            _config = pl.Config;
        }
    }

    public string Command => "giveitem";
    public string[] Aliases => new[] { "gi", "givei", "giveweapon" };
    public string Description => "Donor command to give yourself an item if you have 'giveitem' permission. Checks global limits from config.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var t = DonatorPlugin.Instance?.TranslationData;

        if (!Round.IsStarted)
        {
            response = t?.HelpGiveItemRoundNotStarted ?? "Round not started yet.";
            return false;
        }
        if (!(sender is PlayerCommandSender pcs))
        {
            response = "Only a player can use .giveitem.";
            return false;
        }

        var player = Player.Get(pcs.ReferenceHub);
        if (player == null)
        {
            response = "Failed to find player object.";
            return false;
        }
        var sid = DonatorUtils.CleanSteamId(player.UserId);

        if (!_roleManager.IsDonator(sid))
        {
            response = t?.HelpGiveItemNotDonor ?? "You are not a donor.";
            return false;
        }

        // If user is SCP => forbid
        if (player.Role.Team == PlayerRoles.Team.SCPs)
        {
            response = "You cannot give items to yourself while you are an SCP.";
            return false;
        }

        var roleKey = _roleManager.GetDonatorRole(sid);
        if (!_config.DonatorRoles.TryGetValue(roleKey, out var dRol))
        {
            response = "Your donor role missing in config.";
            return false;
        }
        if (!dRol.Permissions.Contains("giveitem"))
        {
            response = t?.HelpGiveItemNoPerm ?? "You don't have 'giveitem' permission.";
            return false;
        }

        if (!_cooldownManager.CanExecuteCommand(sid, roleKey, "giveitem"))
        {
            response = t?.HelpGiveItemLimit ?? "You reached your giveitem limit.";
            return false;
        }

        if (arguments.Count < 1)
        {
            var all = string.Join(", ", _config.ItemAliases.Keys);
            response = $"{(t?.HelpGiveItemUsage ?? "Usage: .giveitem <alias>")}\nPossible aliases: {all}";
            return false;
        }

        var arg = arguments.At(0).ToLowerInvariant();
        if (_config.ItemAliases.TryGetValue(arg, out var realItem))
            arg = realItem;

        if (!Enum.TryParse(arg, true, out ItemType itemType))
        {
            var suggestion = string.Join(", ", _config.ItemAliases.Keys);
            response = $"Unknown item alias '{arg}'. Possible aliases: {suggestion}";
            return false;
        }

        if (_config.BlacklistedItems.Contains(itemType.ToString()))
        {
            response = t?.HelpGiveItemBlacklisted ?? "That item is blacklisted.";
            return false;
        }

        // give item
        player.AddItem(itemType);
        _cooldownManager.RegisterCommandUsage(sid, roleKey, "giveitem");

        response = $"You received {itemType}.";
        return true;
    }
}
