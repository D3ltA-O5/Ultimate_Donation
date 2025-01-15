using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;
using RemoteAdmin;
using System;

[CommandHandler(typeof(ClientCommandHandler))]
// Либо RemoteAdminCommandHandler, если хотите чтобы вводить через RA,
// но, судя по задаче, это должна быть клиентская команда донатёра.
public class ChangeRoleCommand : ICommand
{
    public string Command => "changerole";
    public string[] Aliases => new[] { "cr" };
    public string Description => "Позволяет донатёру сменить себе роль (класс SCP, охранника и т.п.), учитывая лимиты.";

    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public ChangeRoleCommand(RoleManager roleManager, CooldownManager cooldownManager, Config config)
    {
        _roleManager = roleManager;
        _cooldownManager = cooldownManager;
        _config = config;
    }

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        // Проверим, что команду выполняет именно игрок (не консоль и не сервер)
        if (!(sender is PlayerCommandSender playerSender))
        {
            response = "Команду может выполнить только живой игрок.";
            return false;
        }

        var player = Player.Get(playerSender.ReferenceHub);
        if (player == null)
        {
            response = "Не удалось найти вашего игрока в игре.";
            return false;
        }

        // Проверяем, донатёр ли
        if (!_roleManager.IsDonator(player.UserId))
        {
            response = "У вас нет донат-прав для смены роли.";
            return false;
        }

        // Проверяем, есть ли вообще разрешение на смену роли в списке
        var roleName = _roleManager.GetDonatorRole(player.UserId);
        if (!_config.DonatorRoles.TryGetValue(roleName, out var donatorRole))
        {
            response = "Ваша донат-роль не найдена в конфиге, обратитесь к администратору.";
            return false;
        }

        // Точно ли команда "changerole" есть у этого донатера
        if (!donatorRole.Permissions.Contains("changerole"))
        {
            response = "В вашей донат-роле нет разрешения на смену роли.";
            return false;
        }

        // Проверим лимит по CooldownManager
        if (!_cooldownManager.CanExecuteCommand(player.UserId, roleName, "changerole"))
        {
            response = "Вы исчерпали лимит на смену роли в этом раунде.";
            return false;
        }

        // Должен быть хотя бы 1 аргумент — на какую роль меняемся
        if (arguments.Count < 1)
        {
            response = "Использование: .changerole <роль/класс (число или название)>";
            return false;
        }

        var targetRoleArg = arguments.At(0);

        // Здесь уже ваша логика смены роли. Можно воспользоваться Exiled.API.Features.Player.SetRole(...)
        // Например, отправим игрока за класс D (RoleTypeId.ClassD), если "d" — так далее.
        // Это псевдопример, адаптируйте под свой список.

        var newRole = ParseRole(targetRoleArg);
        if (newRole == RoleTypeId.None)
        {
            response = $"Неизвестная роль: {targetRoleArg}";
            return false;
        }

        // Применяем смену
        player.Role.Set(newRole);

        // Регистрируем использование команды
        _cooldownManager.RegisterCommandUsage(player.UserId, roleName, "changerole");

        response = $"Вы успешно сменили свою роль на {newRole}.";
        return true;
    }

    // Пример вспомогательного метода разбора роли
    private RoleTypeId ParseRole(string arg)
    {
        // Попробуем распарсить как число
        if (int.TryParse(arg, out int roleNumber))
        {
            if (Enum.IsDefined(typeof(RoleTypeId), roleNumber))
                return (RoleTypeId)roleNumber;
        }
        else
        {
            // Или как строку
            foreach (RoleTypeId r in Enum.GetValues(typeof(RoleTypeId)))
            {
                if (r.ToString().Equals(arg, StringComparison.OrdinalIgnoreCase))
                    return r;
            }
        }
        return RoleTypeId.None;
    }
}
