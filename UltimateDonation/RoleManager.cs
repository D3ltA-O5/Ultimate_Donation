using System;
using System.Linq;
using Exiled.API.Features;
using UltimateDonation;

/// <summary>
/// Developer note:
/// Manages donor roles, checks if user is donor, assigns or removes donor from config, etc.
/// Code now reads global usage-limits from Config.GlobalCommandLimits instead of each role object.
/// </summary>
public class RoleManager
{
    private readonly Config _config;
    private readonly DonatorPlugin _plugin;

    public RoleManager(Config config, DonatorPlugin plugin)
    {
        _config = config;
        _plugin = plugin;
    }

    public void AddDonation(string steamId, string role, int days)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);

        if (!_config.DonatorRoles.ContainsKey(role))
        {
            _plugin.LogDebug($"[RoleManager] Attempt to add donation to unknown role '{role}'.");
            throw new ArgumentException($"Role '{role}' not found in config.");
        }

        var expiry = DateTime.Today.AddDays(days);
        var existing = _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);
        if (existing == null)
        {
            var newDon = new PlayerDonation
            {
                Nickname = "Unknown-" + steamId, // or fill manually
                SteamId = steamId,
                Role = role,
                ExpiryDate = expiry
            };
            _config.PlayerDonations.Add(newDon);
        }
        else
        {
            existing.Role = role;
            existing.ExpiryDate = expiry;
        }

        _plugin.LogDebug($"[RoleManager] {steamId} => {role}, expiry={expiry:yyyy-MM-dd} (+{days} days).");
    }

    public void RemoveDonation(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var idx = _config.PlayerDonations.FindIndex(d => d.SteamId == steamId);
        if (idx >= 0)
        {
            _config.PlayerDonations.RemoveAt(idx);
            _plugin.LogDebug($"[RoleManager] Removed donation from {steamId}.");
        }
        else
        {
            _plugin.LogDebug($"[RoleManager] No donation entry for {steamId}.");
        }
    }

    public bool IsDonator(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var don = _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null) return false;
        return don.ExpiryDate.Date >= DateTime.Today;
    }

    public string GetDonatorRole(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var don = _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null) return string.Empty;
        if (don.ExpiryDate.Date >= DateTime.Today) return don.Role;
        return string.Empty;
    }

    public bool IsBlacklistedRole(string roleName)
    {
        return _config.BlacklistedRoles.Contains(roleName.ToLowerInvariant());
    }

    public bool CanChangeToScpYet()
    {
        var time = Round.ElapsedTime.TotalSeconds;
        return time < _config.ScpChangeTimeLimit;
    }

    public int GetDaysLeft(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var don = _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null) return 0;
        var diff = (don.ExpiryDate.Date - DateTime.Today).TotalDays;
        return diff > 0 ? (int)diff : 0;
    }
}
