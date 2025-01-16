using CommandSystem;
using RemoteAdmin; // можно подключить, если нужно
using Exiled.API.Features;
using System;
using UltimateDonation;

[CommandHandler(typeof(ClientCommandHandler))] // <-- ВАЖНО: Команда доступна в клиентской консоли
public class DonatorCommand : ICommand
{
    // Ссылки на ваши менеджеры (RoleManager, Config и т.д.)
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
        // Проверим, что вызвал игрок, а не сервер/RA
        if (!(sender is PlayerCommandSender playerSender))
        {
            response = "This command can only be used by a player in client console.";
            return false;
        }

        // Если аргументов нет, подскажем помощь
        if (arguments.Count == 0)
        {
            response = "Usage: donator <prefix|help> ...";
            return false;
        }

        var subcmd = arguments.At(0).ToLowerInvariant();

        switch (subcmd)
        {
            case "prefix":
                // Ожидаем форму: donator prefix <prefixText> <color>
                if (arguments.Count < 3)
                {
                    response = "Usage: donator prefix <Prefix> <Color>";
                    return false;
                }

                // Проверка, что игрок - донатор, и роль позволяет prefix
                var player = Player.Get(playerSender.ReferenceHub);
                if (player == null)
                {
                    response = "Failed to find your player object.";
                    return false;
                }

                var sid = DonatorUtils.CleanSteamId(player.UserId);
                if (!_roleManager.IsDonator(sid))
                {
                    response = "You are not a donor.";
                    return false;
                }
                var roleKey = _roleManager.GetDonatorRole(sid);
                if (!_config.DonatorRoles.TryGetValue(roleKey, out var dRole))
                {
                    response = "Donor role not found in config.";
                    return false;
                }
                if (!dRole.CustomPrefixEnabled || !_config.CustomPrefixGlobalEnable)
                {
                    response = "This donor role does not allow custom prefixes.";
                    return false;
                }

                // Собираем аргументы
                var prefixValue = arguments.At(1);
                var colorValue = arguments.At(2);

                // Проверяем запрещённые слова (если у вас есть _config.ForbiddenPrefixSubstrings)
                // Пример:
                // if (_config.ForbiddenPrefixSubstrings.Any(bad => prefixValue.Contains(bad, StringComparison.OrdinalIgnoreCase)))
                // {
                //     response = "Your prefix contains a forbidden word.";
                //     return false;
                // }

                // Устанавливаем
                player.RankName = prefixValue;
                player.RankColor = colorValue;

                response = $"Custom prefix '{prefixValue}' color='{colorValue}' set successfully!";
                return true;

            case "help":
            default:
                response =
                    "Donator help:\n" +
                    "- prefix <Prefix> <Color>: Set a custom prefix if your role allows.\n";
                return true;
        }
    }
}
