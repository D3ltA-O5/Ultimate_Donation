[README на русском](README.ru.md)

# UltimateDonation

**UltimateDonation** is a plugin for [SCP: Secret Laboratory](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory) built with the [EXILED](https://github.com/Exiled-Team/EXILED) framework. This plugin is designed to manage donor privileges in a way that **restricts access to administrative controls**, while allowing donors to use controlled commands and features configured by the server administrators. This ensures that donor abuse of admin commands is entirely avoided.

## 📚 Additional Guides for Players

We have created detailed guides for donors that are already formatted for Discord and are perfect for showcasing the server’s donor privileges:

- **[English Guide for Donors](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/eng_guide_for_donaters)**  
- **[Russian Guide for Donors](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/ru_guide_for_donaters)**  

---

## 🎯 Purpose

The primary purpose of **UltimateDonation** is to provide donors with enhanced in-game privileges without compromising server security or administrative integrity. Donors **do not gain access to admin controls or the Remote Admin (RA) panel**. Instead, they are granted access to **carefully controlled commands** with strict limitations, ensuring fair and balanced gameplay.

Key safeguards include:
- **Controlled Commands**: Donors can only use commands explicitly allowed by their roles.
- **Global and Per-Role Limits**: All donor commands have usage limits per round, configurable in the plugin's settings.
- **Blacklist System**: Prevent donors from accessing restricted roles or items.
- **Freezing Mechanisms**: Temporarily suspend donor privileges globally or individually, with compensation for frozen time.

This plugin is ideal for servers that want to reward donors while maintaining strict administrative control.

---

## 🎉 Features

- **Secure Role-Based Privileges**: Assign custom roles to donors with predefined permissions.
- **Controlled Commands for Donors**: Enable donors to use specific commands like `.changerole` or `.giveitem` without access to RA.
- **Global and Individual Freezing**: Freeze donor privileges to temporarily suspend their effects.
- **Usage Limits**: Track and enforce per-round limits on donor command usage.
- **Custom Prefixes**: Allow specific donor roles to set personalized prefixes with custom colors.
- **Blacklist System**: Restrict donors from accessing certain roles or items.
- **Aliases for Convenience**: Shortened command aliases for ease of use.
- **Fully Configurable**: Adjust every aspect of the plugin via configuration and translation files.

---

## 🚀 Installation Instructions

1. **Download** the latest release of `UltimateDonation.dll` from the [Releases](https://github.com/D3ltA-O5//Ultimate_Donation/releases) page.
2. **Place** the `UltimateDonation.dll` file into your server's `EXILED/Plugins` directory.
3. **Restart** your SCP: SL server to generate the configuration and translation files.
4. **Configure** the plugin by editing the exiled config file and `donat_translations.yml` files in your server's `EXILED/Configs` directory.

---

## 📋 Commands

### **For Players**

| Command                  | Aliases              | Description                                                                                 | Example                           |
|---------------------------|----------------------|---------------------------------------------------------------------------------------------|-----------------------------------|
| `.changerole`            | `.cr`, `.role`       | Allows donors to change their role (if allowed by their donor role permissions).            | `.changerole 173`                |
| `.giveitem`              | `.gi`, `.givei`      | Allows donors to give themselves items (if allowed by their donor role permissions).        | `.giveitem rifle`                |
| `.mydon`                 | `.mydonation`, `.md` | Displays information about your donor status, days remaining, and command usage limits.     | `.mydon`                         |
| `.donator prefix`        | N/A                  | Allows donors with permission to set a custom prefix and color.                             | `.donator prefix [VIP] green`    |

### **For Admins**

| Command                     | Description                                                                                     | Example                                  |
|------------------------------|-------------------------------------------------------------------------------------------------|------------------------------------------|
| `donator addrole`           | Assign a donor role to a player for a specific number of days.                                  | `donator addrole 76561199481494871 keter 30` |
| `donator removerole`        | Remove a donor role from a player.                                                              | `donator removerole 76561199481494871`      |
| `donator freezeall`         | Freeze or unfreeze all donations globally.                                                      | `donator freezeall true`                   |
| `donator freezeplayer`      | Freeze or unfreeze a specific player’s donor privileges.                                         | `donator freezeplayer 76561199481494871 true` |
| `donator infoplayer`        | View detailed donor information for a specific player.                                          | `donator infoplayer 76561199481494871`      |
| `donator listroleplayers`   | List all players with a specific donor role.                                                    | `donator listroleplayers keter`            |
| `donator listalldonations`  | Display a list of all donations on the server.                                                  | `donator listalldonations`                 |

---

## 📋 Configuration

The configuration file `Ultimate_Donation.yml` provides extensive customization options:

### **Core Options**
- **`is_enabled`**: Enable or disable the plugin (default: `true`).
- **`debug`**: Enable or disable debug mode for additional logging (default: `false`).

### **Donor Roles**
Define donor roles with the following fields:
- **`name`**: Display name of the role.
- **`badge_color`**: Badge color in the player list.
- **`permissions`**: List of allowed commands (`changerole` or/and `giveitem`).
- **`rank_name`**: Rank name displayed in the player list.
- **`rank_color`**: Color of the rank text.
- **`customprefixenabled`**: Whether the role can set a custom prefix.

### **Limits and Restrictions**
- **`global_command_limits`**: Set per-round usage limits for commands based on roles.
- **`blacklisted_roles`**: List of roles that donors cannot become.
- **`blacklisted_items`**: List of items that donors cannot give themselves.
- **`scp_change_time_limit`**: Time (in seconds) after which donors cannot switch to SCP roles.

### **Customization**
- **`custom_prefix_global_enable`**: Globally enable or disable custom prefixes.

Refer to the example configuration file included in the repository for detailed setup instructions.

---

## 📋 Translation File

The translation file `donat_translations.yml` allows you to customize every message shown to players. 

### **Key Fields**
- **Command Feedback**:
  - `help_donator_command`: Help message for the `donator` command.
  - `help_changerole_usage`: Error message for `.changerole` usage.
  - `help_giveitem_usage`: Error message for `.giveitem` usage.
  - `mydon_status_info`: Template for `.mydon` output, including placeholders for donor role, days remaining, and usage limits.

- **Errors and Warnings**:
  - `only_player_can_use_command`: Error when a non-player tries to use a player-only command.
  - `player_object_not_found`: Error when a player’s object cannot be located.
  - `missing_donor_role_in_config`: Error when a donor role is not defined in the configuration.

The plugin’s messages are fully customizable. Refer to the example translation file in the repository for more details.

---

## 📋 Requirements

- **EXILED Framework**: This plugin requires EXILED version 9.0.0 or higher. Make sure your server is up-to-date.

---

## ⚠️ Important Notes

1. **No Admin Privileges for Donors**:  
   Donors cannot access the Remote Admin (RA) panel or use any commands outside of those explicitly allowed by their roles. This ensures that donors cannot abuse administrative powers.

2. **Strict Limits and Restrictions**:  
   Server administrators have full control over which commands donors can use and how often they can use them. The plugin enforces strict per-round limits to maintain balance.

3. **Customizable Freezing**:  
   Administrators can temporarily freeze donor privileges globally or individually, ensuring complete control over donor interactions.

---

## ✅ What's New

- **v1.0.0**: Initial release of the **UltimateDonation** plugin.
  - Secure donor role management with controlled commands.
  - Support for freezing donor privileges with time compensation.
  - Extensive configuration for roles, limits, and blacklists.
  - Fully customizable messages and behavior via translation and config files.

---

## 📧 Contact and Support

For any questions, issues, or suggestions, please open an issue on the [GitHub repository](https://github.com/D3ltA-O5/Ultimate_Donation) or contact me on Discord: **cyberco**.

---

Thank you for using **UltimateDonation**! Enjoy managing your SCP: SL server securely and efficiently!
