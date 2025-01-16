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
        var t = DonatorPlugin.Instance?.TranslationData;

        // Проверяем, что команду вызвал игрок
        if (!(sender is PlayerCommandSender playerSender))
        {
            // Используем поле MyDonOnlyPlayer из переводов, либо fallback-текст
            response = t?.MyDonOnlyPlayer ?? "This command can only be used by players in the client console.";
            return false;
        }

        // Находим объект игрока
        var player = Player.Get(playerSender.ReferenceHub);
        if (player == null)
        {
            response = t?.PlayerObjectNotFound ?? "Failed to retrieve your player object. Please try again.";
            return false;
        }

        // Очищаем SteamID и проверяем статус донатёра
        var steamId = DonatorUtils.CleanSteamId(player.UserId);
        if (!_roleManager.IsDonator(steamId))
        {
            response = t?.MyDonNotDonor ?? "You are not a donor, or your donation has expired.";
            return false;
        }

        // Проверяем корректность роли
        var roleKey = _roleManager.GetDonatorRole(steamId);
        if (!_config.DonatorRoles.TryGetValue(roleKey, out var donorRole))
        {
            response = t?.MyDonRoleNotConfigured
                ?? "Your donor role is not configured correctly. Please contact an administrator.";
            return false;
        }

        // Сколько дней осталось
        int daysLeft = _roleManager.GetDaysLeft(steamId);

        // Проверяем глобальные лимиты команд
        if (!_config.GlobalCommandLimits.TryGetValue(roleKey, out var commandLimits))
        {
            response = t?.MyDonNoLimitsFound
                ?? $"No global command limits found for your role '{roleKey}'.";
            return false;
        }

        // Составляем информацию об использовании команд
        var usageInfo = commandLimits
            .Select(limit =>
            {
                var commandName = limit.Key;
                var maxUses = limit.Value;
                int used = _cooldownManager.GetUsageCount(steamId, commandName);
                return $"{commandName}: {used}/{maxUses}";
            })
            .ToList();

        // Если нет команд
        string usageSummary = usageInfo.Any()
            ? string.Join("; ", usageInfo)
            : (t?.MyDonNoCommandsTracked ?? "No commands available for usage tracking.");

        // Основной текст (с шаблонами), берём из MyDonStatusInfo
        var textTemplate = t?.MyDonStatusInfo ??
            "=== Your Donation Status ===\n" +
            "- Role: {roleName} (Key: {roleKey})\n" +
            "- Days Left: {daysLeft}\n" +
            "- Permissions: {permissions}\n" +
            "- Command Usage This Round: {usageSummary}\n" +
            "(Tip: Use '.changerole' or '.giveitem' if allowed by your role.)";

        // Заменяем шаблоны {roleName}, {roleKey}, {daysLeft}, {permissions}, {usageSummary}
        response = textTemplate
            .Replace("{roleName}", donorRole.Name)
            .Replace("{roleKey}", roleKey)
            .Replace("{daysLeft}", daysLeft.ToString())
            .Replace("{permissions}", string.Join(", ", donorRole.Permissions))
            .Replace("{usageSummary}", usageSummary);

        return true;
    }
}
