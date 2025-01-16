using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using System;
using InventorySystem.Items;

[CommandHandler(typeof(ClientCommandHandler))]
public class GiveItemCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public GiveItemCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _cooldownManager = plugin.CooldownManager;
            _config = plugin.Config;
        }
    }

    public string Command => "giveitem";
    public string[] Aliases => new[] { "gi" };
    public string Description => "Allows a donor to give themselves an item (limited usage).";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        DonatorPlugin.Instance?.LogDebug($"[GiveItemCommand] Executing giveitem, args={string.Join(" ", arguments)}");

        if (!(sender is PlayerCommandSender ps))
        {
            response = "This command can only be used by a player.";
            return false;
        }

        var player = Player.Get(ps.ReferenceHub);
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

        if (donatorRole.permissions == null || !donatorRole.permissions.Contains("giveitem"))
        {
            response = "You do not have permission to give items.";
            return false;
        }

        if (!_cooldownManager.CanExecuteCommand(player.UserId, roleName, "giveitem"))
        {
            response = "You have reached the usage limit for item giving this round.";
            return false;
        }

        if (arguments.Count < 1)
        {
            response = "Usage: .giveitem <ItemType>";
            return false;
        }

        var itemTypeStr = arguments.At(0);
        if (!Enum.TryParse(itemTypeStr, true, out ItemType parsedItem))
        {
            response = $"Unknown ItemType: {itemTypeStr}";
            return false;
        }

        player.AddItem(parsedItem);
        _cooldownManager.RegisterCommandUsage(player.UserId, roleName, "giveitem");

        response = $"You gave yourself item: {parsedItem}.";
        return true;
    }
}
