using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using PluginAPI.Roles;

public class DiscordManager
{
    private readonly DiscordSocketClient _client;
    private readonly PermissionsManager _permissionsManager;
    private readonly RoleManager _roleManager;

    private readonly string _token;
    private readonly ulong _channelId;

    public DiscordManager(string token, ulong channelId, PermissionsManager permissionsManager, RoleManager roleManager)
    {
        _token = token;
        _channelId = channelId;
        _permissionsManager = permissionsManager;
        _roleManager = roleManager;

        _client = new DiscordSocketClient();
        _client.Log += LogAsync;
        _client.MessageReceived += MessageReceivedAsync;
    }

    public async Task StartAsync()
    {
        await _client.LoginAsync(TokenType.Bot, _token);
        await _client.StartAsync();
    }

    public async Task StopAsync()
    {
        await _client.StopAsync();
    }

    private Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());
        return Task.CompletedTask;
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Channel.Id != _channelId || message.Author.IsBot)
            return;

        var args = message.Content.Split(' ');
        if (args.Length < 3) return;

        var command = args[0];
        var steamId = args[1];
        var role = args[2];
        var days = args.Length > 3 ? int.Parse(args[3]) : 0;

        if (command == "!addrole")
        {
            _roleManager.AddDonation(steamId, role, days);
            await message.Channel.SendMessageAsync($"Роль {role} успешно добавлена для игрока {steamId} на {days} дней.");
        }
    }
}
