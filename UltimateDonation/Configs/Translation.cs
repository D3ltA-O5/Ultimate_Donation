using Exiled.API.Interfaces;
using YamlDotNet.Serialization;
using System.Collections.Generic;

namespace UltimateDonation
{
    public sealed class Translation : ITranslation
    {
        [YamlMember(Alias = "help_donator_command")]
        public string HelpDonatorCommand { get; set; } = "Donator plugin help.";

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
        public string AliasesNote { get; set; } = "You can define aliases in the translation file.";

        [YamlMember(Alias = "only_player_can_use_command")]
        public string OnlyPlayerCanUseCommand { get; set; } = "Only a player can use this command (not console).";

        [YamlMember(Alias = "player_object_not_found")]
        public string PlayerObjectNotFound { get; set; } = "Failed to retrieve your player object. Please try again.";

        [YamlMember(Alias = "missing_donor_role_in_config")]
        public string MissingDonorRoleInConfig { get; set; } = "Your donor role is missing in config.";

        [YamlMember(Alias = "unknown_role_alias")]
        public string UnknownRoleAlias { get; set; } = "Unknown role alias/id";

        [YamlMember(Alias = "change_role_success")]
        public string ChangeRoleSuccess { get; set; } = "You changed your role to {roleName}.";

        [YamlMember(Alias = "cannot_give_item_as_scp")]
        public string CannotGiveItemAsScp { get; set; } = "You cannot give items while you're an SCP.";

        [YamlMember(Alias = "unknown_item_alias")]
        public string UnknownItemAlias { get; set; } = "Unknown item alias";

        [YamlMember(Alias = "give_item_success")]
        public string GiveItemSuccess { get; set; } = "You received {itemType}.";

        [YamlMember(Alias = "mydon_only_player")]
        public string MyDonOnlyPlayer { get; set; } = "This command can only be used by players in the client console.";

        [YamlMember(Alias = "mydon_not_donor")]
        public string MyDonNotDonor { get; set; } = "You are not a donor, or your donation has expired.";

        [YamlMember(Alias = "mydon_role_not_configured")]
        public string MyDonRoleNotConfigured { get; set; } = "Your donor role is not configured correctly. Please contact an administrator.";

        [YamlMember(Alias = "mydon_no_limits_found")]
        public string MyDonNoLimitsFound { get; set; } = "No global command limits found for your role '{roleKey}'.";

        [YamlMember(Alias = "mydon_no_commands_tracked")]
        public string MyDonNoCommandsTracked { get; set; } = "No commands available for usage tracking.";

        [YamlMember(Alias = "mydon_status_info")]
        public string MyDonStatusInfo { get; set; } =
            "=== Your Donation Status ===\n" +
            "- Role: {roleName} (Key: {roleKey})\n" +
            "- Days Left: {daysLeft}\n" +
            "- Permissions: {permissions}\n" +
            "- Command Usage This Round: {usageSummary}\n" +
            "(Tip: Use '.changerole' or '.giveitem' if allowed by your role.)";

        [YamlMember(Alias = "donator_only_players")]
        public string DonatorOnlyPlayers { get; set; } = "This command can only be used by a player in client console.";

        [YamlMember(Alias = "prefix_not_allowed")]
        public string PrefixNotAllowed { get; set; } = "This donor role does not allow custom prefixes.";

        [YamlMember(Alias = "prefix_set_success")]
        public string PrefixSetSuccess { get; set; } = "Custom prefix '{prefixValue}' color='{colorValue}' set successfully!";

        [YamlMember(Alias = "prefix_forbidden_word_message")]
        public string PrefixForbiddenWordMessage { get; set; } = "Your prefix contains a forbidden word!";

        [YamlMember(Alias = "role_aliases")]
        public Dictionary<string, string> RoleAliases { get; set; } = new Dictionary<string, string>();

        [YamlMember(Alias = "item_aliases")]
        public Dictionary<string, string> ItemAliases { get; set; } = new Dictionary<string, string>();
    }
}
