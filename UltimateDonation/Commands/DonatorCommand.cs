using CommandSystem;
using RemoteAdmin;  
using Exiled.API.Features;
using System;
using System.Linq;
using UltimateDonation;

[CommandHandler(typeof(ClientCommandHandler))] 
public class DonatorCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly Config _config;

    public DonatorCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _config = plugin.Config;
        }
    }

    public string Command => "donator";
    public string[] Aliases => new[] { "don" };
    public string Description => "Manages donor functionalities (prefix, etc.) for client console.";

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
            response = t.DonatorOnlyPlayers;
            return false;
        }

        if (arguments.Count == 0)
        {
            response = "Usage: donator <prefix|help> ...";
            return false;
        }

        var subcmd = arguments.At(0).ToLowerInvariant();

        switch (subcmd)
        {
            case "prefix":

                if (arguments.Count < 3)
                {
                    response = t.HelpPrefixUsage;
                    return false;
                }

                var player = Player.Get(playerSender.ReferenceHub);
                if (player == null)
                {
                    response = t.PlayerObjectNotFound;
                    return false;
                }

                var sid = DonatorUtils.CleanSteamId(player.UserId);
                if (!_roleManager.IsDonator(sid))
                {
                    response = t.HelpChangeRoleNotDonor;
                    return false;
                }

                var roleKey = _roleManager.GetDonatorRole(sid);
                if (!_config.DonatorRoles.TryGetValue(roleKey, out var dRole))
                {
                    response = t.MissingDonorRoleInConfig;
                    return false;
                }

                if (!dRole.CustomPrefixEnabled || !_config.CustomPrefixGlobalEnable)
                {
                    response = t.PrefixNotAllowed;
                    return false;
                }

                var prefixValue = arguments.At(1);
                var colorValue = arguments.At(2);

                if (_config.ForbiddenPrefixSubstrings.Any(bad =>
                    prefixValue.IndexOf(bad, StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    response = t.PrefixForbiddenWordMessage;
                    return false;
                }

                player.RankName = prefixValue;
                player.RankColor = colorValue;

                response = t.PrefixSetSuccess
                    .Replace("{prefixValue}", prefixValue)
                    .Replace("{colorValue}", colorValue);
                return true;

            case "help":
            default:
                response =
                    "Donator help:\n" +
                    "- prefix <Prefix> <Color>: Set a custom prefix if your role allows.\n" +
                    "- (More subcommands can be added in the future).";
                return true;
        }
    }
}
