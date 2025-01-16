using YamlDotNet.Serialization;

/// <summary>
/// Developer note:
/// English text for help messages & such. 
/// Each property matches a key in 'donat_translations.yml'.
/// </summary>
public class TranslationData
{
    [YamlMember(Alias = "help_donator_command")]
    public string HelpDonatorCommand { get; set; } = "Donator plugin help. (no YAML)";

    [YamlMember(Alias = "help_changerole_usage")]
    public string HelpChangeRoleUsage { get; set; } = "Usage: .changerole <RoleAlias>";

    [YamlMember(Alias = "help_changerole_not_donor")]
    public string HelpChangeRoleNotDonor { get; set; } = "You are not a donor.";

    [YamlMember(Alias = "help_changerole_round_not_started")]
    public string HelpChangeRoleRoundNotStarted { get; set; } = "The round hasn't started yet.";

    [YamlMember(Alias = "help_changerole_no_perm")]
    public string HelpChangeRoleNoPerm { get; set; } = "No permission to change roles.";

    [YamlMember(Alias = "help_changerole_limit")]
    public string HelpChangeRoleLimit { get; set; } = "You reached your changerole limit.";

    [YamlMember(Alias = "help_changerole_blacklisted")]
    public string HelpChangeRoleBlacklisted { get; set; } = "This role is blacklisted.";

    [YamlMember(Alias = "help_changerole_scp_timed_out")]
    public string HelpChangeRoleScpTimedOut { get; set; } = "Too late to become SCP.";

    [YamlMember(Alias = "help_changerole_scp_already_exists")]
    public string HelpChangeRoleScpAlreadyExists { get; set; } = "That SCP already exists.";

    [YamlMember(Alias = "help_giveitem_usage")]
    public string HelpGiveItemUsage { get; set; } = "Usage: .giveitem <ItemAlias>";

    [YamlMember(Alias = "help_giveitem_round_not_started")]
    public string HelpGiveItemRoundNotStarted { get; set; } = "You cannot use .giveitem before round starts.";

    [YamlMember(Alias = "help_giveitem_not_donor")]
    public string HelpGiveItemNotDonor { get; set; } = "You are not a donor.";

    [YamlMember(Alias = "help_giveitem_no_perm")]
    public string HelpGiveItemNoPerm { get; set; } = "No permission to give items.";

    [YamlMember(Alias = "help_giveitem_limit")]
    public string HelpGiveItemLimit { get; set; } = "You reached your item-giving limit.";

    [YamlMember(Alias = "help_giveitem_blacklisted")]
    public string HelpGiveItemBlacklisted { get; set; } = "That item is blacklisted.";

    [YamlMember(Alias = "help_prefix_usage")]
    public string HelpPrefixUsage { get; set; } = "Usage: donator prefix <SteamId> <Prefix> <Color>";

    [YamlMember(Alias = "aliases_note")]
    public string AliasesNote { get; set; } = "You can define aliases in UltimateDonation.yaml (role_aliases, item_aliases).";
}
