using System;
using System.Linq;
using Exiled.API.Features;
using UltimateDonation;
using System.Collections.Generic;

public class RoleManager
{
    private readonly Config _config;
    private readonly DonatorPlugin _plugin;

    private bool _allDonationsFrozen = false;
    private DateTime? _globalFreezeStartedAt = null;

    public bool AllDonationsFrozen
    {
        get => _allDonationsFrozen;
        set
        {
            if (_allDonationsFrozen == value)
                return;

            if (value)
            {
                _globalFreezeStartedAt = DateTime.UtcNow;
            }
            else
            {
                if (_globalFreezeStartedAt.HasValue)
                {
                    var frozenDuration = DateTime.UtcNow - _globalFreezeStartedAt.Value;

                    foreach (var don in _plugin.DonationsManager.DonationsData.PlayerDonations)
                    {
                        if (!don.IsFrozen)
                        {
                            don.ExpiryDate = don.ExpiryDate.Add(frozenDuration);
                        }
                    }
                    _plugin.DonationsManager.SaveDonationsData();
                }
                _globalFreezeStartedAt = null;
            }

            _allDonationsFrozen = value;
        }
    }

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

        var donationsList = _plugin.DonationsManager.DonationsData.PlayerDonations;

        var existing = donationsList.FirstOrDefault(d => d.SteamId == steamId);
        if (existing == null)
        {
            var newDon = new PlayerDonation
            {
                Nickname = "Unknown-" + steamId,
                SteamId = steamId,
                Role = role,
                ExpiryDate = expiry
            };
            donationsList.Add(newDon);
        }
        else
        {
            existing.Role = role;
            existing.ExpiryDate = expiry;
        }


        _plugin.DonationsManager.SaveDonationsData();

        _plugin.LogDebug($"[RoleManager] {steamId} => {role}, expiry={expiry:yyyy-MM-dd} (+{days} days).");
    }

    public void RemoveDonation(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);

        var donationsList = _plugin.DonationsManager.DonationsData.PlayerDonations;
        var idx = donationsList.FindIndex(d => d.SteamId == steamId);
        if (idx >= 0)
        {
            donationsList.RemoveAt(idx);
            _plugin.LogDebug($"[RoleManager] Removed donation from {steamId}.");

            _plugin.DonationsManager.SaveDonationsData();
        }
        else
        {
            _plugin.LogDebug($"[RoleManager] No donation entry for {steamId}.");
        }
    }

    public bool IsDonator(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        if (_allDonationsFrozen)
            return false;

        var donationsList = _plugin.DonationsManager.DonationsData.PlayerDonations;
        var don = donationsList.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null) return false;

        if (don.IsFrozen)
            return false;

        return don.ExpiryDate.Date >= DateTime.Today;
    }

    public string GetDonatorRole(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);

        var donationsList = _plugin.DonationsManager.DonationsData.PlayerDonations;
        var don = donationsList.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null)
            return string.Empty;

        if (_allDonationsFrozen)
            return string.Empty;
        if (don.IsFrozen)
            return string.Empty;
        if (don.ExpiryDate.Date >= DateTime.Today)
            return don.Role;

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

        var donationsList = _plugin.DonationsManager.DonationsData.PlayerDonations;
        var don = donationsList.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null)
            return 0;

        var diff = (don.ExpiryDate.Date - DateTime.Today).TotalDays;
        return diff > 0 ? (int)diff : 0;
    }

    public PlayerDonation GetDonationInfo(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);

        var donationsList = _plugin.DonationsManager.DonationsData.PlayerDonations;
        return donationsList.FirstOrDefault(d => d.SteamId == steamId);
    }

    public List<PlayerDonation> GetAllDonations()
    {
        return _plugin.DonationsManager.DonationsData.PlayerDonations;
    }

    public List<PlayerDonation> GetDonationsByRole(string roleKey)
    {
        roleKey = roleKey.ToLowerInvariant();
        var donationsList = _plugin.DonationsManager.DonationsData.PlayerDonations;
        return donationsList
            .Where(d => d.Role.Equals(roleKey, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public void SetDonationFrozen(string steamId, bool freezeValue)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var don = GetDonationInfo(steamId);
        if (don == null)
            return;

        if (freezeValue)
        {
            if (!don.IsFrozen)
            {
                don.IsFrozen = true;
                don.FreezeStartedAt = DateTime.UtcNow;
            }
        }
        else
        {
            if (don.IsFrozen && don.FreezeStartedAt.HasValue)
            {
                var frozenDuration = DateTime.UtcNow - don.FreezeStartedAt.Value;
                don.ExpiryDate = don.ExpiryDate.Add(frozenDuration);
            }

            don.IsFrozen = false;
            don.FreezeStartedAt = null;
        }

        _plugin.DonationsManager.SaveDonationsData();
    }
}
