using System;
using System.Linq;
using Exiled.API.Features;
using UltimateDonation;
using System.Collections.Generic;

/// <summary>
/// Developer note:
/// Manages donor roles, checks if user is donor, assigns or removes donor from config, etc.
/// Дополнительно теперь учитывает механику заморозки и компенсирует время при разморозке.
/// </summary>
public class RoleManager
{
    private readonly Config _config;
    private readonly DonatorPlugin _plugin;

    /// <summary>
    /// Флаг глобальной заморозки всех донатов.
    /// Если true - никто не считается донатором, пока не будет снята заморозка.
    /// </summary>
    private bool _allDonationsFrozen = false;

    /// <summary>
    /// Время начала глобальной заморозки (null, если не активна).
    /// </summary>
    private DateTime? _globalFreezeStartedAt = null;

    /// <summary>
    /// Глобальная заморозка: если установить в true - все донаты «выключены»,
    /// а при возврате к false - мы компенсируем время, проведённое в заморозке,
    /// добавив его к дате истечения донатов (которые не были заморожены индивидуально).
    /// </summary>
    public bool AllDonationsFrozen
    {
        get => _allDonationsFrozen;
        set
        {
            // Если состояние не меняется, ничего не делаем
            if (_allDonationsFrozen == value)
                return;

            if (value)
            {
                // Начинаем глобальную заморозку
                _globalFreezeStartedAt = DateTime.UtcNow;
            }
            else
            {
                // Снимаем глобальную заморозку => компенсируем время для всех не-замороженных индивидуально
                if (_globalFreezeStartedAt.HasValue)
                {
                    var frozenDuration = DateTime.UtcNow - _globalFreezeStartedAt.Value;
                    foreach (var don in _config.PlayerDonations)
                    {
                        // Если у игрока нет индивидуальной заморозки, то прибавляем замороженное время
                        if (!don.IsFrozen)
                        {
                            don.ExpiryDate = don.ExpiryDate.Add(frozenDuration);
                        }
                    }
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

    /// <summary>
    /// Добавляет или продлевает донат игроку.
    /// </summary>
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
                Nickname = "Unknown-" + steamId, // или указать вручную
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
            // При желании здесь можно автоматически сбрасывать заморозку (existing.IsFrozen = false),
            // если нужно, чтобы продление доната снимало заморозку. Но оставим так.
        }

        _plugin.LogDebug($"[RoleManager] {steamId} => {role}, expiry={expiry:yyyy-MM-dd} (+{days} days).");
    }

    /// <summary>
    /// Удаляет донат игрока (если есть).
    /// </summary>
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

    /// <summary>
    /// Проверяет, является ли игрок донатором с учётом даты истечения и флагов заморозки.
    /// </summary>
    public bool IsDonator(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);

        // Если активна глобальная заморозка, то никто не считается донатором
        if (_allDonationsFrozen)
            return false;

        var don = _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null)
            return false;

        // Если игрок индивидуально заморожен
        if (don.IsFrozen)
            return false;

        return don.ExpiryDate.Date >= DateTime.Today;
    }

    /// <summary>
    /// Возвращает ключ донатной роли (например, "keter") у игрока,
    /// или пустую строку, если донат истёк / заморожен / нет записи.
    /// </summary>
    public string GetDonatorRole(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var don = _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);

        if (don == null)
            return string.Empty;

        // Проверяем глобальную заморозку
        if (_allDonationsFrozen)
            return string.Empty;

        // Проверяем индивидуальную заморозку
        if (don.IsFrozen)
            return string.Empty;

        // Проверяем дату
        if (don.ExpiryDate.Date >= DateTime.Today)
            return don.Role;

        return string.Empty;
    }

    /// <summary>
    /// Проверка на чёрный список ролей.
    /// </summary>
    public bool IsBlacklistedRole(string roleName)
    {
        return _config.BlacklistedRoles.Contains(roleName.ToLowerInvariant());
    }

    /// <summary>
    /// Можно ли ещё менять роль на SCP, или время вышло?
    /// </summary>
    public bool CanChangeToScpYet()
    {
        var time = Round.ElapsedTime.TotalSeconds;
        return time < _config.ScpChangeTimeLimit;
    }

    /// <summary>
    /// Сколько дней осталось донату у данного SteamID (если доната нет или он истёк — вернёт 0).
    /// </summary>
    public int GetDaysLeft(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var don = _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);
        if (don == null)
            return 0;

        var diff = (don.ExpiryDate.Date - DateTime.Today).TotalDays;
        return diff > 0 ? (int)diff : 0;
    }

    /// <summary>
    /// Получить всю информацию о донате конкретного игрока (или null).
    /// </summary>
    public PlayerDonation GetDonationInfo(string steamId)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        return _config.PlayerDonations.FirstOrDefault(d => d.SteamId == steamId);
    }

    /// <summary>
    /// Список всех донатов в конфиге.
    /// </summary>
    public List<PlayerDonation> GetAllDonations()
    {
        return _config.PlayerDonations;
    }

    /// <summary>
    /// Возвращает список всех донатеров (записей), у кого Role == roleKey (без учёта истёк / не истёк).
    /// </summary>
    public List<PlayerDonation> GetDonationsByRole(string roleKey)
    {
        roleKey = roleKey.ToLowerInvariant();
        return _config.PlayerDonations
            .Where(d => d.Role.Equals(roleKey, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    // ----------------------------------------------------------------------------
    // Новый метод для индивидуальной заморозки / разморозки доната:
    // ----------------------------------------------------------------------------
    /// <summary>
    /// Устанавливает флаг заморозки для конкретного игрока. При разморозке
    /// компенсирует время, добавляя его к ExpiryDate.
    /// </summary>
    public void SetDonationFrozen(string steamId, bool freezeValue)
    {
        steamId = DonatorUtils.CleanSteamId(steamId);
        var don = GetDonationInfo(steamId);
        if (don == null)
            return;

        // Если замораживаем
        if (freezeValue)
        {
            // Ставим флаг и запоминаем время, когда началась заморозка
            if (!don.IsFrozen)
            {
                don.IsFrozen = true;
                don.FreezeStartedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // Снимаем заморозку
            if (don.IsFrozen && don.FreezeStartedAt.HasValue)
            {
                var frozenDuration = DateTime.UtcNow - don.FreezeStartedAt.Value;
                don.ExpiryDate = don.ExpiryDate.Add(frozenDuration);
            }

            don.IsFrozen = false;
            don.FreezeStartedAt = null;
        }
    }
}
