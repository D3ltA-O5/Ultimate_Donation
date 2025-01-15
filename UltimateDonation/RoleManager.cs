using System;
using System.Collections.Generic;

public class RoleManager
{
    private readonly Config _config;

    public RoleManager(Config config)
    {
        _config = config;
    }

    public void AddDonation(string steamId, string role, int days)
    {
        if (!_config.DonatorRoles.ContainsKey(role))
            throw new ArgumentException("Роль не существует.");

        _config.PlayerDonations[steamId] = new PlayerDonation
        {
            SteamId = steamId,
            Role = role,
            ExpiryDate = DateTime.Now.AddDays(days)
        };
    }

    public void RemoveDonation(string steamId)
    {
        _config.PlayerDonations.Remove(steamId);
    }

    public bool IsDonator(string steamId)
    {
        return _config.PlayerDonations.ContainsKey(steamId) &&
               _config.PlayerDonations[steamId].ExpiryDate > DateTime.Now;
    }

    public string GetDonatorRole(string steamId)
    {
        return _config.PlayerDonations.ContainsKey(steamId) ? _config.PlayerDonations[steamId].Role : string.Empty;
    }
}
