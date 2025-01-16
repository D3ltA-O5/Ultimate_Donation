using System;
using Exiled.API.Features;

public class RoleManager
{
    private readonly Config _config;
    private readonly DonatorPlugin _plugin;

    public RoleManager(Config config, DonatorPlugin plugin)
    {
        _config = config;
        _plugin = plugin;
    }

    // Добавить донат игроку <steamId> с ролью <role> на days дней
    public void AddDonation(string steamId, string role, int days)
    {
        var rolesDict = _config.UltimateDonation.donator_roles;
        if (!rolesDict.ContainsKey(role))
        {
            _plugin.LogDebug($"[RoleManager] Attempt to add donation for {steamId}, but role '{role}' isn't in donator_roles.");
            throw new ArgumentException($"Role {role} does not exist in config.");
        }

        var expiry = DateTime.Now.AddDays(days);

        _config.UltimateDonation.player_donations[steamId] = new PlayerDonation
        {
            steam_id = steamId,
            role = role,
            expiry_date = expiry
        };

        _plugin.LogDebug($"[RoleManager] Player {steamId} got donor role '{role}' until {expiry}.");
    }

    // Удалить донат-роль
    public void RemoveDonation(string steamId)
    {
        if (_config.UltimateDonation.player_donations.Remove(steamId))
            _plugin.LogDebug($"[RoleManager] Donor role removed from {steamId}.");
        else
            _plugin.LogDebug($"[RoleManager] Tried to remove donor role from {steamId}, but entry not found.");
    }

    // Проверяем, активна ли донат-роль
    public bool IsDonator(string steamId)
    {
        var dict = _config.UltimateDonation.player_donations;
        if (!dict.TryGetValue(steamId, out var donation))
            return false;

        bool active = donation.expiry_date > DateTime.Now;
        _plugin.LogDebug($"[RoleManager] IsDonator({steamId})? role={donation.role}, expiry={donation.expiry_date}, active={active}");
        return active;
    }

    public string GetDonatorRole(string steamId)
    {
        var dict = _config.UltimateDonation.player_donations;
        if (!dict.TryGetValue(steamId, out var donation))
            return string.Empty;

        if (donation.expiry_date <= DateTime.Now)
            return string.Empty;

        return donation.role; // e.g. "vip" or "premium"
    }
}
