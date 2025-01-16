using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;
using Exiled.API.Features;

public class PermissionsManager
{
    private readonly string _filePath;
    private Dictionary<string, object> _permissionsData;
    private readonly DonatorPlugin _plugin;

    public PermissionsManager(string filePath, DonatorPlugin plugin)
    {
        _filePath = filePath;
        _plugin = plugin;
        LoadPermissions();
    }

    private void LoadPermissions()
    {
        if (!File.Exists(_filePath))
        {
            _permissionsData = new Dictionary<string, object>();
            _plugin.LogDebug($"[PermissionsManager] File {_filePath} not found. Will create on save.");
            return;
        }

        var yaml = File.ReadAllText(_filePath);
        var deserializer = new DeserializerBuilder().Build();
        _permissionsData = deserializer.Deserialize<Dictionary<string, object>>(yaml)
                           ?? new Dictionary<string, object>();

        _plugin.LogDebug($"[PermissionsManager] Loaded {_filePath}, top-level keys={_permissionsData.Count}.");
    }

    public void SavePermissions()
    {
        var serializer = new SerializerBuilder().Build();
        File.WriteAllText(_filePath, serializer.Serialize(_permissionsData));
        _plugin.LogDebug($"[PermissionsManager] Saved file {_filePath}.");
    }

    // Прописываем роли и их permissions в отдельные ключи, чтобы не путаться с EXILED.Permissions
    public void UpdateRolesAndPermissions(Dictionary<string, DonatorRole> donatorRoles)
    {
        _permissionsData["DonatorRolesMeta"] = new List<string>();
        var rolesSection = (List<string>)_permissionsData["DonatorRolesMeta"];

        rolesSection.Clear();
        rolesSection.AddRange(donatorRoles.Keys);

        _permissionsData["DonatorPermsMap"] = new Dictionary<string, object>();
        var permsMap = (Dictionary<string, object>)_permissionsData["DonatorPermsMap"];
        permsMap.Clear();

        foreach (var kvp in donatorRoles)
        {
            var roleName = kvp.Key;
            var roleData = kvp.Value;

            // Проходим по всем permissions
            if (roleData.permissions == null)
                continue;

            foreach (var perm in roleData.permissions)
            {
                if (!permsMap.ContainsKey(perm))
                    permsMap[perm] = new List<string>();

                var rolesWithPerm = (List<string>)permsMap[perm];
                if (!rolesWithPerm.Contains(roleName))
                    rolesWithPerm.Add(roleName);
            }
        }

        _plugin.LogDebug($"[PermissionsManager] Updated DonatorRolesMeta & DonatorPermsMap with {donatorRoles.Count} roles.");
        SavePermissions();
    }

    // Прописываем user: SteamId@steam -> Role
    public void UpdateMembers(Dictionary<string, PlayerDonation> playerDonations)
    {
        _permissionsData["user"] = new Dictionary<string, object>();
        var membersSection = (Dictionary<string, object>)_permissionsData["user"];
        membersSection.Clear();

        foreach (var donation in playerDonations.Values)
        {
            var key = $"{donation.steam_id}@steam";
            membersSection[key] = donation.role;
        }

        _plugin.LogDebug($"[PermissionsManager] Updated user section. {playerDonations.Count} donors recorded.");
        SavePermissions();
    }
}
