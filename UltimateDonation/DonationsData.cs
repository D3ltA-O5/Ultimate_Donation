using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

public class DonationsData
{
    [YamlMember(Alias = "player_donations")]
    public List<PlayerDonation> PlayerDonations { get; set; } = new List<PlayerDonation>();
}
