using CommandSystem;
using RemoteAdmin;
using Exiled.API.Features;
using System;
using System.Linq;
using UltimateDonation;

[CommandHandler(typeof(ClientCommandHandler))] // Команда доступна в клиентской консоли
public class MyDonCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public MyDonCommand()
    {
        var plugin = DonatorPlugin.Instance;
        if (plugin != null)
        {
            _roleManager = plugin.RoleManager;
            _cooldownManager = plugin.CooldownManager;
            _config = plugin.Config;
        }
        else
        {
            Log.Error("MyDonCommand: DonatorPlugin instance is null. Ensure plugin initialization.");
        }
    }

    public string Command => "mydon";
    public string[] Aliases => new[] { "mydonation", "mydn" };
    public string Description => "Check your donor status, days left, and command usage limits.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // Проверяем, что команду вызвал игрок
        if (!(sender is PlayerCommandSender playerSender))
        {
            response = "This command can only be used by players in the client console.";
            return false;
        }

        // Находим объект игрока
        var player = Player.Get(playerSender.ReferenceHub);
        if (player == null)
        {
            response = "Failed to retrieve your player object. Please try again.";
            return false;
        }

        // Очищаем SteamID и проверяем статус донатёра
        var steamId = DonatorUtils.CleanSteamId(player.UserId);
        if (!_roleManager.IsDonator(steamId))
        {
            response = "You are not a donor, or your donation has expired.";
            return false;
        }

        // Проверяем роль донатёра
        var roleKey = _roleManager.GetDonatorRole(steamId);
        if (!_config.DonatorRoles.TryGetValue(roleKey, out var donorRole))
        {
            response = "Your donor role is not configured correctly. Please contact an administrator.";
            return false;
        }

        // Вычисляем оставшиеся дни
        int daysLeft = _roleManager.GetDaysLeft(steamId);

        // Проверяем глобальные ограничения команд
        if (!_config.GlobalCommandLimits.TryGetValue(roleKey, out var commandLimits))
        {
            response = $"No global command limits found for your role '{roleKey}'.";
            return false;
        }

        // Формируем информацию об использовании команд
        var usageInfo = commandLimits
            .Select(limit =>
            {
                var command = limit.Key;
                var maxUses = limit.Value;
                int used = _cooldownManager.GetUsageCount(steamId, command);
                return $"{command}: {used}/{maxUses}";
            })
            .ToList();

        string usageSummary = usageInfo.Any()
            ? string.Join("; ", usageInfo)
            : "No commands available for usage tracking.";

        // Формируем финальный ответ для игрока
        response =
            $"=== Your Donation Status ===\n" +
            $"- Role: {donorRole.Name} (Key: {roleKey})\n" +
            $"- Days Left: {daysLeft}\n" +
            $"- Permissions: {string.Join(", ", donorRole.Permissions)}\n" +
            $"- Command Usage This Round: {usageSummary}\n" +
            "(Tip: Use '.changerole' or '.giveitem' if allowed by your role.)";

        return true;
    }
}
