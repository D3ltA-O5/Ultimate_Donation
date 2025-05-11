using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using Exiled.Permissions.Extensions; // <-- подключено
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
    public string Description => "Donor command to change your role if allowed.";

    // Добавлено для корректного вывода текста с <..> (не удалялись форм. символы).
    public bool SanitizeResponse => false;

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var t = DonatorPlugin.Instance?.Translation as Translation;
        if (t == null)
        {
            response = "Translation not loaded.";
            return false;
        }

        // Проверка прав "donator.changerole"
        if (!sender.CheckPermission("donator.changerole"))
        {
            response = "You don't have permission to use changerole. Required: donator.changerole";
            return false;
        }

        if (!Round.IsStarted)
        {
            response = t.HelpChangeRoleRoundNotStarted;
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
            response = t.HelpChangeRoleNotDonor;
            return false;
        }

        var roleKey = _roleManager.GetDonatorRole(sid);
        if (!_config.DonatorRoles.TryGetValue(roleKey, out var dRol))
        {
            response = t.MissingDonorRoleInConfig;
            return false;
        }

        if (!dRol.Permissions.Contains("changerole"))
        {
            response = t.HelpChangeRoleNoPerm;
            return false;
        }

        if (!_cooldownManager.CanExecuteCommand(sid, roleKey, "changerole"))
        {
            response = t.HelpChangeRoleLimit;
            return false;
        }

        if (arguments.Count < 1)
        {
            var allAliases = (t.RoleAliases != null)
                ? string.Join(", ", t.RoleAliases.Keys)
                : "No aliases found.";
            response = $"{t.HelpChangeRoleUsage}\nPossible aliases: {allAliases}";
            return false;
        }

        var arg = arguments.At(0).ToLowerInvariant();

        if (t.RoleAliases.TryGetValue(arg, out var realRole))
            arg = realRole;

        var parsedRole = ParseRole(arg);
        if (parsedRole == RoleTypeId.None)
        {
            var suggestion = (t.RoleAliases != null)
                ? string.Join(", ", t.RoleAliases.Keys)
                : "No aliases available.";
            response = $"{t.UnknownRoleAlias} '{arg}'. Possible aliases: {suggestion}";
            return false;
        }

        if (_roleManager.IsBlacklistedRole(parsedRole.ToString().ToLowerInvariant()))
        {
            response = t.HelpChangeRoleBlacklisted;
            return false;
        }

        if (parsedRole.ToString().StartsWith("Scp", StringComparison.OrdinalIgnoreCase))
        {
            if (!_roleManager.CanChangeToScpYet())
            {
                response = t.HelpChangeRoleScpTimedOut;
                return false;
            }
            if (Player.List.Any(pl => pl.Role == parsedRole))
            {
                response = t.HelpChangeRoleScpAlreadyExists;
                return false;
            }
        }

        player.Role.Set(parsedRole);
        _cooldownManager.RegisterCommandUsage(sid, roleKey, "changerole");

        response = t.ChangeRoleSuccess.Replace("{roleName}", parsedRole.ToString());
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
