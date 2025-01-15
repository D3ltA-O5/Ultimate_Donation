using Discord.WebSocket;
using Discord;
using System.Threading.Tasks;
using System;

public class DiscordManager
{
    private readonly DiscordSocketClient _client;
    private readonly RoleManager _roleManager;
    private readonly string _token;
    private readonly ulong _guildId;
    private readonly ulong _channelId;

    public DiscordManager(string token, ulong guildId, ulong channelId, RoleManager roleManager)
    {
        _token = token;
        _guildId = guildId;
        _channelId = channelId;
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
        // Игнорируем сообщения не из нужного канала и от ботов
        if (message.Channel.Id != _channelId || message.Author.IsBot)
            return;

        var args = message.Content.Split(' ');
        if (args.Length == 0) return;

        var command = args[0].ToLower();

        // Пример команды "!addrole STEAM_0:1:12345 vip 10"
        if (command == "!addrole")
        {
            if (args.Length < 4)
            {
                await message.Channel.SendMessageAsync("Использование: !addrole <SteamID> <RoleName> <Days>");
                return;
            }

            var steamId = args[1];
            var role = args[2];
            if (!int.TryParse(args[3], out int days))
            {
                await message.Channel.SendMessageAsync("Days должно быть числом.");
                return;
            }

            _roleManager.AddDonation(steamId, role, days);
            await message.Channel.SendMessageAsync($"Роль {role} добавлена игроку {steamId} на {days} дн.");
        }
        else if (command == "!removerole")
        {
            if (args.Length < 2)
            {
                await message.Channel.SendMessageAsync("Использование: !removerole <SteamID>");
                return;
            }

            var steamId = args[1];
            _roleManager.RemoveDonation(steamId);
            await message.Channel.SendMessageAsync($"С {steamId} снята донат-роль.");
        }
        // Можете добавить "!adddays", "!checklimits" и т. д.
    }
}
