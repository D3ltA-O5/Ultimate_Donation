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
    public string Description => "Donor command to give yourself an item if you have 'giveitem' permission.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var t = DonatorPlugin.Instance?.Translation as Translation;
        if (t == null)
        {
            response = "Translation not loaded.";
            return false;
        }

        if (!Round.IsStarted)
        {
            response = t.HelpGiveItemRoundNotStarted;
            return false;
        }
        if (!(sender is PlayerCommandSender pcs))
        {
            response = t.OnlyPlayerCanUseCommand;
            return false;
        }

        var player = Player.Get(pcs.ReferenceHub);
        if (player == null)
        {
            response = t.PlayerObjectNotFound;
            return false;
        }

        var sid = DonatorUtils.CleanSteamId(player.UserId);
        if (!_roleManager.IsDonator(sid))
        {
            response = t.HelpGiveItemNotDonor;
            return false;
        }

        if (player.Role.Team == PlayerRoles.Team.SCPs)
        {
            response = t.CannotGiveItemAsScp;
            return false;
        }

        var roleKey = _roleManager.GetDonatorRole(sid);
        if (!_config.DonatorRoles.TryGetValue(roleKey, out var dRol))
        {
            response = t.MissingDonorRoleInConfig;
            return false;
        }
        if (!dRol.Permissions.Contains("giveitem"))
        {
            response = t.HelpGiveItemNoPerm;
            return false;
        }

        if (!_cooldownManager.CanExecuteCommand(sid, roleKey, "giveitem"))
        {
            response = t.HelpGiveItemLimit;
            return false;
        }

        if (arguments.Count < 1)
        {
            var allAliases = (t.ItemAliases != null)
                ? string.Join(", ", t.ItemAliases.Keys)
                : "No item aliases defined.";
            response = $"{t.HelpGiveItemUsage}\nPossible aliases: {allAliases}";
            return false;
        }

        var arg = arguments.At(0).ToLowerInvariant();

        if (t.ItemAliases.TryGetValue(arg, out var realItem))
            arg = realItem;

        if (!Enum.TryParse(arg, true, out ItemType itemType))
        {
            var suggestion = (t.ItemAliases != null)
                ? string.Join(", ", t.ItemAliases.Keys)
                : "No aliases available.";
            response = $"{t.UnknownItemAlias} '{arg}'. Possible aliases: {suggestion}";
            return false;
        }

        if (_config.BlacklistedItems.Contains(itemType.ToString()))
        {
            response = t.HelpGiveItemBlacklisted;
            return false;
        }

        player.AddItem(itemType);
        _cooldownManager.RegisterCommandUsage(sid, roleKey, "giveitem");

        response = t.GiveItemSuccess.Replace("{itemType}", itemType.ToString());
        return true;
    }
}
