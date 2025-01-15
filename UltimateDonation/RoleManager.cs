using System;

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
            throw new ArgumentException("Роль не существует в конфиге.");

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
        if (!_config.PlayerDonations.TryGetValue(steamId, out var donation))
            return string.Empty;

        // Проверим, не истёк ли донат
        if (donation.ExpiryDate <= DateTime.Now)
            return string.Empty;

        return donation.Role;
    }
}
