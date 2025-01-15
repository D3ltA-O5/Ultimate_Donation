using CommandSystem;
using PluginAPI.Roles;
using System;

public class RemoveRoleCommand : ICommand
{
    private readonly RoleManager _roleManager;

    public RemoveRoleCommand(RoleManager roleManager)
    {
        _roleManager = roleManager;
    }

    public string Command => "removerole";

    public string[] Aliases => new[] { "remover" };

    public string Description => "Удалить роль донатера.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (arguments.Count < 1)
        {
            response = "Использование: donator removerole <SteamID>";
            return false;
        }

        var targetId = arguments.At(0);
        _roleManager.RemoveDonation(targetId);
        response = $"Роль игрока {targetId} успешно удалена.";
        return true;
    }
}
