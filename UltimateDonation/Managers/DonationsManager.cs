using System;
using System.IO;
using Exiled.API.Features;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class DonationsManager
{
    private readonly string _donationsFilePath;
    private readonly DonatorPlugin _plugin;

    public DonationsData DonationsData { get; private set; } = new DonationsData();

    public DonationsManager(DonatorPlugin plugin)
    {
        _plugin = plugin;

        _donationsFilePath = Path.Combine(Paths.Configs, "DonationsData.yml");

        LoadDonationsData();
    }

    public void LoadDonationsData()
    {
        try
        {
            if (!File.Exists(_donationsFilePath))
            {
                _plugin.LogDebug($"[DonationsManager] File not found: {_donationsFilePath}. Creating a new empty one.");
                SaveDonationsData(); 
            }
            else
            {
                var yaml = File.ReadAllText(_donationsFilePath);
                var ds = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                var data = ds.Deserialize<DonationsData>(yaml);
                if (data != null)
                {
                    DonationsData = data;
                    _plugin.LogDebug($"[DonationsManager] Loaded {DonationsData.PlayerDonations.Count} donations from file.");
                }
                else
                {
                    _plugin.LogDebug("[DonationsManager] Deserialized data is null, using empty list.");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error($"[DonationsManager] Failed to load donations data: {ex}");
        }
    }

    public void SaveDonationsData()
    {
        try
        {
            var sb = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            string yaml = sb.Serialize(DonationsData);
            File.WriteAllText(_donationsFilePath, yaml);

            _plugin.LogDebug($"[DonationsManager] Donations data saved to {_donationsFilePath}.");
        }
        catch (Exception ex)
        {
            Log.Error($"[DonationsManager] Failed to save donations data: {ex}");
        }
    }
}
