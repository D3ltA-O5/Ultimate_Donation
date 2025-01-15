using CommandSystem;
using Exiled.API.Features;
using System;
using RemoteAdmin;
using InventorySystem.Items;

[CommandHandler(typeof(ClientCommandHandler))]
public class GiveItemCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public GiveItemCommand(RoleManager roleManager, CooldownManager cooldownManager, Config config)
    {
        _roleManager = roleManager;
        _cooldownManager = cooldownManager;
        _config = config;
    }

    public string Command => "giveitem";
    public string[] Aliases => new[] { "gi" };
    public string Description => "Выдать предмет донатёру (только себе, если так задумано).";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!(sender is PlayerCommandSender playerSender))
        {
            response = "Команду может использовать только игрок.";
            return false;
        }

        var player = Player.Get(playerSender.ReferenceHub);
        if (player == null)
        {
            response = "Не удалось найти игрока.";
            return false;
        }

        if (!_roleManager.IsDonator(player.UserId))
        {
            response = "У вас нет донат-прав для выдачи предметов.";
            return false;
        }

        var roleName = _roleManager.GetDonatorRole(player.UserId);
        if (!_config.DonatorRoles.TryGetValue(roleName, out var donatorRole))
        {
            response = "Ваша донат-роль не найдена в конфиге.";
            return false;
        }

        // Проверяем, есть ли разрешение на giveitem
        if (!donatorRole.Permissions.Contains("giveitem"))
        {
            response = "В вашей донат-роле нет разрешения на выдачу предметов.";
            return false;
        }

        // Проверим лимит на usage
        if (!_cooldownManager.CanExecuteCommand(player.UserId, roleName, "giveitem"))
        {
            response = "Вы исчерпали лимит на выдачу предметов в этом раунде.";
            return false;
        }

        // Для выдачи **себе** достаточно 1 аргумента: что за предмет
        if (arguments.Count < 1)
        {
            response = "Использование: .giveitem <ItemType>";
            return false;
        }

        var itemTypeStr = arguments.At(0);

        if (!Enum.TryParse(itemTypeStr, true, out ItemType parsedItem))
        {
            response = $"Неизвестный ItemType: {itemTypeStr}";
            return false;
        }

        // Выдаём предмет
        player.AddItem(parsedItem);

        // Регистрируем использование
        _cooldownManager.RegisterCommandUsage(player.UserId, roleName, "giveitem");

        response = $"Вы выдали себе предмет {parsedItem}.";
        return true;
    }
}
