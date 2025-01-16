using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using System;
using System.Linq;
using PlayerRoles;
using UltimateDonation;

[CommandHandler(typeof(ClientCommandHandler))]
public class ChangeRoleCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public ChangeRoleCommand()
    {
        var pl = DonatorPlugin.Instance;
        if (pl != null)
        {
            _roleManager = pl.RoleManager;
            _cooldownManager = pl.CooldownManager;
            _config = pl.Config;
        }
    }

    public string Command => "changerole";
    public string[] Aliases => new[] { "cr", "role", "chrole" };
    public string Description => "Donor command to change your role if allowed (no duplication of command-limits in role).";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var t = DonatorPlugin.Instance?.TranslationData;

        if (!Round.IsStarted)
        {
            response = t?.HelpChangeRoleRoundNotStarted ?? "The round hasn't started.";
            return false;
        }

        if (!(sender is PlayerCommandSender pcs))
        {
            response = "Only a player can use this command (not console).";
            return false;
        }

        var player = Player.Get(pcs.ReferenceHub);
        if (player == null)
        {
            response = "Failed to find your player object.";
            return false;
        }

        var sid = DonatorUtils.CleanSteamId(player.UserId);
        if (!_roleManager.IsDonator(sid))
        {
            response = t?.HelpChangeRoleNotDonor ?? "You are not a donor.";
            return false;
        }

        var roleKey = _roleManager.GetDonatorRole(sid);
        if (!_config.DonatorRoles.TryGetValue(roleKey, out var dRol))
        {
            response = "Your donor role is missing in config.";
            return false;
        }
        // Must have "changerole" permission
        if (!dRol.Permissions.Contains("changerole"))
        {
            response = t?.HelpChangeRoleNoPerm ?? "You don't have permission to change roles.";
            return false;
        }
        // Check usage limit from global
        if (!_cooldownManager.CanExecuteCommand(sid, roleKey, "changerole"))
        {
            response = t?.HelpChangeRoleLimit ?? "You reached the changerole limit this round.";
            return false;
        }

        if (arguments.Count < 1)
        {
            var allAliases = string.Join(", ", _config.RoleAliases.Keys);
            response = $"{(t?.HelpChangeRoleUsage ?? "Usage: .changerole <roleAlias>")}\nPossible aliases: {allAliases}";
            return false;
        }

        var arg = arguments.At(0).ToLowerInvariant();
        if (_config.RoleAliases.TryGetValue(arg, out var realRole))
            arg = realRole;

        var parsedRole = ParseRole(arg);
        if (parsedRole == RoleTypeId.None)
        {
            var suggestion = string.Join(", ", _config.RoleAliases.Keys);
            response = $"Unknown role alias/id '{arg}'. Possible aliases: {suggestion}";
            return false;
        }

        // blacklist
        if (_roleManager.IsBlacklistedRole(parsedRole.ToString().ToLowerInvariant()))
        {
            response = t?.HelpChangeRoleBlacklisted ?? "That role is blacklisted.";
            return false;
        }

        // if scp => time check, duplication check
        if (parsedRole.ToString().StartsWith("Scp", StringComparison.OrdinalIgnoreCase))
        {
            if (!_roleManager.CanChangeToScpYet())
            {
                response = t?.HelpChangeRoleScpTimedOut ?? "Too late to become SCP.";
                return false;
            }
            if (Player.List.Any(pl => pl.Role == parsedRole))
            {
                response = t?.HelpChangeRoleScpAlreadyExists ?? "That SCP already exists.";
                return false;
            }
        }

        // Set role
        player.Role.Set(parsedRole);
        _cooldownManager.RegisterCommandUsage(sid, roleKey, "changerole");

        response = $"You changed your role to {parsedRole}.";
        return true;
    }

    private RoleTypeId ParseRole(string s)
    {
        if (int.TryParse(s, out int val))
        {
            if (Enum.IsDefined(typeof(RoleTypeId), val))
                return (RoleTypeId)val;
        }
        else
        {
            foreach (RoleTypeId rt in Enum.GetValues(typeof(RoleTypeId)))
            {
                if (rt.ToString().Equals(s, StringComparison.OrdinalIgnoreCase))
                    return rt;
            }
        }
        return RoleTypeId.None;
    }
}
