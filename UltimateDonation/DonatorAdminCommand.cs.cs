using CommandSystem;
using Exiled.API.Features;
using System;
using UltimateDonation;

[CommandHandler(typeof(RemoteAdminCommandHandler))] // ключ к доступу из RA
public class DonatorAdminCommand : ICommand
{
    // Ссылки на менеджеры (RoleManager, CooldownManager, Config, и т.д.)
    private readonly RoleManager _roleManager;
    private readonly Config _config;

    // Название команды, алиасы, описание
    public string Command => "donator";
    public string[] Aliases => new[] { "donadm", "dadm" };
    public string Description => "Admin command to manage donor roles (addrole, removerole, etc.).";

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

        // Если нет аргументов — покажем помощь
        if (arguments.Count < 1)
        {
            response = "Usage: donator <addrole|removerole> <SteamID64> <roleKey> <days>\n" +
                       "Example: donator addrole 76561199... keter 30";
            return false;
        }

        var subCommand = arguments.At(0).ToLowerInvariant();
        switch (subCommand)
        {
            case "addrole":
                {
                    // donator addrole <steamId> <role> <days>
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

            default:
                {
                    // Неизвестная подкоманда
                    response = "Unknown subcommand. Use 'addrole' or 'removerole'.";
                    return false;
                }
        }
    }
}
