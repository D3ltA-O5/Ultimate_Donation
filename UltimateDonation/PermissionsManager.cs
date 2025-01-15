using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

public class PermissionsManager
{
    private readonly string _filePath;
    private Dictionary<string, object> _permissionsData;

    public PermissionsManager(string filePath)
    {
        _filePath = filePath;
        LoadPermissions();
    }

    private void LoadPermissions()
    {
        if (!File.Exists(_filePath))
        {
            _permissionsData = new Dictionary<string, object>();
            return;
        }

        var yaml = File.ReadAllText(_filePath);
        var deserializer = new DeserializerBuilder().Build();
        _permissionsData = deserializer.Deserialize<Dictionary<string, object>>(yaml) ?? new Dictionary<string, object>();
    }

    public void SavePermissions()
    {
        var serializer = new SerializerBuilder().Build();
        File.WriteAllText(_filePath, serializer.Serialize(_permissionsData));
    }

    public void UpdateRolesAndPermissions(Dictionary<string, DonatorRole> donatorRoles)
    {
        // Секция "Roles" в permissions.yml
        if (!_permissionsData.ContainsKey("Roles"))
            _permissionsData["Roles"] = new List<string>();

        var rolesSection = (List<string>)_permissionsData["Roles"];
        rolesSection.Clear();
        rolesSection.AddRange(donatorRoles.Keys);

        // Секция "permissions"
        if (!_permissionsData.ContainsKey("permissions"))
            _permissionsData["permissions"] = new Dictionary<string, object>();

        var permissionsSection = (Dictionary<string, object>)_permissionsData["permissions"];
        permissionsSection.Clear();

        // Для каждой команды (permission) складываем список ролей, у которых она есть
        foreach (var role in donatorRoles)
        {
            foreach (var permission in role.Value.Permissions)
            {
                if (!permissionsSection.ContainsKey(permission))
                    permissionsSection[permission] = new List<string>();

                var rolesWithPermission = (List<string>)permissionsSection[permission];
                if (!rolesWithPermission.Contains(role.Key))
                    rolesWithPermission.Add(role.Key);
            }
        }

        SavePermissions();
    }

    public void UpdateMembers(Dictionary<string, PlayerDonation> playerDonations)
    {
        // Секция "user"
        if (!_permissionsData.ContainsKey("user"))
            _permissionsData["user"] = new Dictionary<string, object>();

        var membersSection = (Dictionary<string, object>)_permissionsData["user"];
        membersSection.Clear();

        // Привязываем каждого донатёра к его роли
        foreach (var donation in playerDonations.Values)
        {
            membersSection[$"{donation.SteamId}@steam"] = donation.Role;
        }

        SavePermissions();
    }
}
