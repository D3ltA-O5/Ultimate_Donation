using CommandSystem;
using RemoteAdmin;
using System;
using Exiled.API.Features;
using PluginAPI.Roles;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class DonatorCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public DonatorCommand(RoleManager roleManager, CooldownManager cooldownManager, Config config)
    {
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _cooldownManager = cooldownManager ?? throw new ArgumentNullException(nameof(cooldownManager));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public string Command => "donator";
    public string[] Aliases => new[] { "don" };
    public string Description => "Управление донатными привилегиями (для админов через RA).";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // Например, RA-админ может прописать: donator addrole <SteamID> <RoleName> <Days>
        // или donator removerole <SteamID>
        // или donator checklimits <SteamID>
        // Для упрощения просто покажем мини-диспетчер:

        if (arguments.Count == 0)
        {
            response = "Использование: donator <addrole|removerole|checklimits> ...";
            return false;
        }

        var subCmd = arguments.At(0).ToLower();
        switch (subCmd)
        {
            case "addrole":
                // Можно переиспользовать AddRoleCommand напрямую, либо просто скопировать логику
                // Ниже — короткая версия
                if (arguments.Count < 4)
                {
                    response = "Использование: donator addrole <SteamID> <RoleName> <Days>";
                    return false;
                }
                var steamId = arguments.At(1);
                var roleName = arguments.At(2);
                if (!int.TryParse(arguments.At(3), out var days))
                {
                    response = "Days должно быть числом.";
                    return false;
                }
                if (!_config.DonatorRoles.ContainsKey(roleName))
                {
                    response = $"Роль {roleName} не найдена в конфиге.";
                    return false;
                }
                _roleManager.AddDonation(steamId, roleName, days);
                response = $"Игроку {steamId} добавлена роль {roleName} на {days} дней.";
                return true;

            case "removerole":
                if (arguments.Count < 2)
                {
                    response = "Использование: donator removerole <SteamID>";
                    return false;
                }
                steamId = arguments.At(1);
                _roleManager.RemoveDonation(steamId);
                response = $"С {steamId} снята донат-роль.";
                return true;

            case "checklimits":
                if (arguments.Count < 2)
                {
                    response = "Использование: donator checklimits <SteamID>";
                    return false;
                }
                steamId = arguments.At(1);
                if (!_roleManager.IsDonator(steamId))
                {
                    response = "У игрока нет активной донат-роли.";
                    return false;
                }
                var r = _roleManager.GetDonatorRole(steamId);
                response = _config.DonatorRoles.ContainsKey(r)
                    ? $"Лимиты для роли {r}: смена роли {_config.DonatorRoles[r].RoleChangeLimit} раз, выдача предметов {_config.DonatorRoles[r].ItemGiveLimit} раз, и т. п."
                    : $"Не найдена роль {r} в конфиге.";
                return true;

            default:
                response = "Неизвестная субкоманда.";
                return false;
        }
    }
}
