using System;
using System.IO;
using System.Collections.Generic; // <-- чтобы использовать List<>
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

        // Создаём поддиректорию "Ultimate_Donation" в EXILED/Configs, если нет
        string donationFolder = Path.Combine(Paths.Configs, "Ultimate_Donation");
        Directory.CreateDirectory(donationFolder);

        // Путь для DonationsData.yml в этой новой папке:
        _donationsFilePath = Path.Combine(donationFolder, "DonationsData.yml");

        LoadDonationsData();
    }

    public void LoadDonationsData()
    {
        try
        {
            if (!File.Exists(_donationsFilePath))
            {
                _plugin.LogDebug($"[DonationsManager] File not found: {_donationsFilePath}. Creating a new sample...");

                // Создадим тестовые (пример) записи, чтобы user видел структуру
                var sampleList = new List<PlayerDonation>
                {
                    new PlayerDonation
                    {
                        Nickname = "JohnDoeExample",
                        SteamId = "76561199000000000",
                        Role = "safe",
                        ExpiryDate = DateTime.Today.AddDays(7),
                        IsFrozen = false
                    },
                    new PlayerDonation
                    {
                        Nickname = "AliceTester",
                        SteamId = "76561198123456789",
                        Role = "keter",
                        ExpiryDate = DateTime.Today.AddDays(30),
                        IsFrozen = true,
                        FreezeStartedAt = DateTime.UtcNow.AddDays(-1) // пример
                    }
                };

                DonationsData.PlayerDonations = sampleList;
                SaveDonationsData(); // Сразу сохраним как пример
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
