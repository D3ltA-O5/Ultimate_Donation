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

    public string Description => "Управление донатными привилегиями.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // Проверяем, что отправитель — игрок
        if (!(sender is PlayerCommandSender playerSender))
        {
            response = "Эту команду может выполнить только игрок.";
            return false;
        }

        // Получаем объект Player для отправителя
        var player = Player.Get(playerSender.ReferenceHub);
        if (player == null)
        {
            response = "Не удалось найти игрока.";
            return false;
        }

        // Проверяем, является ли игрок донатором
        if (!_roleManager.IsDonator(player.UserId))
        {
            response = "Вы не являетесь донатором.";
            return false;
        }

        // Получаем роль игрока
        var roleName = _roleManager.GetDonatorRole(player.UserId);
        if (!_config.DonatorRoles.TryGetValue(roleName, out var donatorRole))
        {
            response = "Роль донатора не найдена.";
            return false;
        }

        // Проверяем аргументы команды
        if (arguments.Count == 0)
        {
            response = "Не указана команда. Используйте: /donator <команда>";
            return false;
        }

        var commandName = arguments.At(0);

        // Проверяем разрешения
        if (!donatorRole.Permissions.Contains(commandName))
        {
            response = "У вас недостаточно прав для выполнения этой команды.";
            return false;
        }

        // Проверяем лимиты на выполнение команды
        if (!_cooldownManager.CanExecuteCommand(player.UserId, commandName))
        {
            response = "Команда находится на перезарядке. Попробуйте позже.";
            return false;
        }

        // Регистрируем использование команды
        _cooldownManager.RegisterCommandUsage(player.UserId, commandName);

        // Если команда успешно выполнена
        response = $"Команда '{commandName}' успешно выполнена!";
        return true;
    }
}
