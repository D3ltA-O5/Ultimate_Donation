using CommandSystem;
using RemoteAdmin;
using System;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class RemoveRoleCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly Config _config;

    public RemoveRoleCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _config = plugin.Config;
        }
    }

    public string Command => "removerole";
    public string[] Aliases => new[] { "remover" };
    public string Description => "Remove a donor role from a player.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        DonatorPlugin.Instance?.LogDebug($"[RemoveRoleCommand] Executing removerole, args={string.Join(" ", arguments)}");

        if (arguments.Count < 1)
        {
            response = "Usage: donator removerole <SteamID>";
            return false;
        }

        var steamId = arguments.At(0);
        _roleManager.RemoveDonation(steamId);

        response = $"Donor role removed from {steamId}.";
        return true;
    }
}
