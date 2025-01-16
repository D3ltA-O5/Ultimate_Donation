using CommandSystem;
using RemoteAdmin; // можно подключить, если нужно
using Exiled.API.Features;
using System;
using System.Linq;
using UltimateDonation;

[CommandHandler(typeof(RemoteAdminCommandHandler))] // ключ к доступу из RA
public class DonatorAdminCommand : ICommand
{
    // Ссылки на менеджеры (RoleManager, Config, и т.д.)
    private readonly RoleManager _roleManager;
    private readonly Config _config;

    // Название команды, алиасы, описание
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

    // Основная логика
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // Проверим, что мы действительно вызываем из RA (не обязательно, но полезно)
        if (!(sender is CommandSender))
        {
            response = "You must run this command in RA console.";
            return false;
        }

        // Если нет аргументов — показываем помощь
        if (arguments.Count < 1)
        {
            response = GetHelpText();
            return false;
        }

        var subCommand = arguments.At(0).ToLowerInvariant();
        switch (subCommand)
        {
            // ---------------------
            // Добавление донат-роли
            // ---------------------
            case "addrole":
                {
                    // donator addrole <steamId> <roleKey> <days>
                    if (arguments.Count < 4)
                    {
                        response = "Usage: donator addrole <SteamID64> <roleKey> <days>";
                        return false;
                    }

                    var steamId = arguments.At(1);
                    var roleKey = arguments.At(2).ToLowerInvariant(); // например, "keter"
                    if (!int.TryParse(arguments.At(3), out var days))
                    {
                        response = "Days must be an integer (number of days).";
                        return false;
                    }

                    // Проверка, есть ли роль в конфиге
                    if (!_config.DonatorRoles.ContainsKey(roleKey))
                    {
                        response = $"Role '{roleKey}' not found in config DonatorRoles.";
                        return false;
                    }

                    // Добавляем
                    _roleManager.AddDonation(steamId, roleKey, days);

                    response = $"Donator role '{roleKey}' added to {steamId} for {days} days.";
                    return true;
                }

            // ---------------------
            // Удаление донат-роли
            // ---------------------
            case "removerole":
                {
                    // donator removerole <steamId>
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

            // ---------------------
            // Заморозить/разморозить все донаты
            // ---------------------
            case "freezeall":
                {
                    // donator freezeall <true|false>
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

            // ---------------------
            // Заморозить/разморозить донат одного игрока
            // ---------------------
            case "freezeplayer":
                {
                    // donator freezeplayer <steamId> <true|false>
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

                    // Вызываем метод, который учитывает компенсацию времени
                    _roleManager.SetDonationFrozen(steamId, freezeValue);

                    response = freezeValue
                        ? $"Donation for {steamId} is now FROZEN (individually)."
                        : $"Donation for {steamId} is now UNFROZEN (individually).";
                    return true;
                }

            // ---------------------
            // Получить инфо о донате одного игрока
            // ---------------------
            case "infoplayer":
                {
                    // donator infoplayer <steamId>
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

            // ---------------------
            // Список игроков с конкретной донатной ролью
            // ---------------------
            case "listroleplayers":
                {
                    // donator listroleplayers <roleKey>
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

            // ---------------------
            // Список всех донатов на сервере
            // ---------------------
            case "listalldonations":
                {
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

            // ---------------------
            // Помощь
            // ---------------------
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
