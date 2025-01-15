using CommandSystem;
using PluginAPI.Roles;
using System;

public class GiveItemCommand : ICommand
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;

    public GiveItemCommand(RoleManager roleManager, CooldownManager cooldownManager)
    {
        _roleManager = roleManager;
        _cooldownManager = cooldownManager;
    }

    public string Command => "giveitem";

    public string[] Aliases => new[] { "gi" };

    public string Description => "Выдать предмет донатеру.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count < 2)
        {
            response = "Использование: giveitem <SteamID> <ItemType>";
            return false;
        }

        var targetId = arguments.At(0);
        var itemType = arguments.At(1);

        if (!_roleManager.IsDonator(targetId))
        {
            response = "Игрок не имеет донатной роли.";
            return false;
        }

        // Здесь добавьте выдачу предмета игроку
        response = $"Предмет {itemType} выдан игроку {targetId}.";
        return true;
    }
}
