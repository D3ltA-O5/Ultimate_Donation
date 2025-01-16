[README на русском](README.ru.md)

# UltimateDonation

**UltimateDonation** is a plugin for [SCP: Secret Laboratory](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory) built with the [EXILED](https://github.com/Exiled-Team/EXILED) framework. This plugin provides donor management functionality while **restricting access to administrative controls**, ensuring secure and balanced gameplay. All donor data is now dynamically stored and managed in a `DonationsData` file, which is updated in real time.

## 📚 Additional Guides for Players

We have created detailed guides for donors, formatted for Discord and perfect for showcasing donor privileges:

- **[English Guide for Donors](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/eng_guide_for_donaters)**  
- **[Russian Guide for Donors](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/ru_guide_for_donaters)**  

---

## 🎯 Purpose

The main purpose of **UltimateDonation** is to provide in-game privileges to donors while maintaining strict administrative control. Donors **cannot access administrative tools or Remote Admin (RA)**. Instead, they gain access to **strictly defined commands** with configurable limits. 

Key improvements:
- **Dynamic Data Storage**: Donor information is stored in a dedicated `DonationsData` file, separate from the EXILED configuration.
- **Real-Time Updates**: Administrative actions (e.g., adding or removing roles) update the `DonationsData` file instantly.
- **Configurable Roles**: Donor roles are defined and configured within the main EXILED configuration.
- **RA Integration**: Full control over donor privileges directly from the Remote Admin console.

---

## 🎉 Features

- **Dynamic Donor Management**: Donor data is dynamically stored in `DonationsData` for real-time updates.
- **Controlled Commands**: Allow donors to use commands like `.changerole` or `.giveitem` with strict permissions and limits.
- **RA Command Integration**: Manage donor roles, freezing, and privileges directly from the RA console.
- **Custom Prefixes**: Enable certain donor roles to set personalized prefixes and colors.
- **Global and Individual Freezing**: Temporarily suspend donor privileges globally or individually.
- **Blacklist System**: Restrict access to specific roles or items.
- **Usage Limits**: Enforce per-round usage limits for donor commands.
- **Aliases for Commands**: Define role and item aliases for ease of use.
- **Translation Support**: Customize all plugin messages through `donat_translations.yml`.

---

## 🚀 Installation Instructions

1. **Download** the latest release of `UltimateDonation.dll` from the [Releases](https://github.com/D3ltA-O5/Ultimate_Donation/releases) page.
2. **Place** the `UltimateDonation.dll` file into your server's `EXILED/Plugins` directory.
3. **Restart** your SCP: SL server to generate the config, `donat_translations.yml` and `DonationsData.json` files.
4. **Configure Roles**: Edit the donor role configuration in your main EXILED config file.
5. **Manage Translations**: Customize player-facing messages in the `donat_translations.yml` file.

---

## 📋 Commands

### **For Players**

| Command                  | Aliases              | Description                                                                                 | Example                           |
|---------------------------|----------------------|---------------------------------------------------------------------------------------------|-----------------------------------|
| `.changerole`            | `.cr`, `.role`       | Allows donors to change their role (if permitted).                                          | `.changerole 173`                |
| `.giveitem`              | `.gi`, `.givei`      | Allows donors to give themselves items (if permitted).                                      | `.giveitem rifle`                |
| `.mydon`                 | `.mydonation`, `.md` | Displays donor status, days remaining, and command usage limits.                            | `.mydon`                         |
| `.donator prefix`        | N/A                  | Allows eligible donors to set a custom prefix and color.                                    | `.donator prefix [VIP] green`    |

### **For Admins (RA)**

| Command                     | Description                                                                                     | Example                                  |
|------------------------------|-------------------------------------------------------------------------------------------------|------------------------------------------|
| `donator addrole`           | Assign a donor role to a player for a specific number of days.                                  | `donator addrole 76561199481494871 keter 30` |
| `donator removerole`        | Remove a donor role from a player.                                                              | `donator removerole 76561199481494871`      |
| `donator freezeall`         | Freeze or unfreeze all donor privileges globally.                                               | `donator freezeall true`                   |
| `donator freezeplayer`      | Freeze or unfreeze donor privileges for a specific player.                                      | `donator freezeplayer 76561199481494871 true` |
| `donator infoplayer`        | View detailed donor information for a specific player.                                          | `donator infoplayer 76561199481494871`      |
| `donator listroleplayers`   | List all players with a specific donor role.                                                    | `donator listroleplayers keter`            |
| `donator listalldonations`  | Display a list of all donations on the server.                                                  | `donator listalldonations`                 |

---

## 📋 Configuration

### **Donor Role Configuration**
Donor roles are now defined in the main EXILED configuration file. Each role includes the following properties:
- **`name`**: Display name of the role.
- **`badge_color`**: Badge color in the player list.
- **`permissions`**: List of allowed commands (e.g., `changerole`, `giveitem`).
- **`rank_name`**: Rank name displayed in the player list.
- **`rank_color`**: Color of the rank text.
- **`customprefixenabled`**: Whether the role allows custom prefixes.

### **Dynamic Donor Data**
All donor data, such as player SteamIDs, roles, and expiration dates, is stored in a dynamically updated `DonationsData.json` file. Administrative actions from RA (e.g., adding or removing donor roles) automatically update this file.

### **Limits and Restrictions**
- **`global_command_limits`**: Configure per-round limits for donor commands.
- **`blacklisted_roles`**: List roles that donors cannot switch to.
- **`blacklisted_items`**: List items that donors cannot give themselves.
- **`scp_change_time_limit`**: Time (in seconds) after which donors cannot switch to SCP roles.

---

## 📋 Translation File

The `donat_translations.yml` file allows you to customize all player-facing messages.

### **Key Fields**
- **Command Feedback**:
  - `help_donator_command`: Help message for the `donator` command.
  - `help_changerole_usage`: Usage message for `.changerole`.
  - `help_giveitem_usage`: Usage message for `.giveitem`.
  - `mydon_status_info`: Template for `.mydon` output.

- **Errors and Warnings**:
  - `only_player_can_use_command`: Error when a non-player uses a player-only command.
  - `player_object_not_found`: Error when a player's object cannot be retrieved.
  - `missing_donor_role_in_config`: Error when a donor role is not configured.

---

## 📋 Requirements

- **EXILED Framework**: This plugin requires EXILED version 9.0.0 or higher. Ensure your server is up-to-date.

---

## ⚠️ Important Notes

1. **Dynamic Donor Management**: All donor data is stored in `DonationsData.json` and updated automatically by RA commands.
2. **No Admin Privileges for Donors**: Donors cannot access RA or administrative functions, ensuring balanced gameplay.
3. **RA Command Integration**: Administrators can fully manage donors through the Remote Admin console.
4. **Customizable Messages**: Use `donat_translations.yml` to adjust all messages displayed to players.

---

## ✅ What's New

- **Dynamic Donor Data**: Real-time updates to donor data stored in `DonationsData.json`.
- **Integrated Role Management**: Roles are defined in the main EXILED configuration file.
- **Enhanced RA Commands**: Full control over donor roles and privileges through RA.
- **v1.1.0**: Latest improvements and optimizations.

---

## 📧 Contact and Support

For any questions, issues, or suggestions, please open an issue on the [GitHub repository](https://github.com/D3ltA-O5/Ultimate_Donation) or contact me on Discord: **cyberco**.

---

Thank you for using **UltimateDonation**! Enjoy managing your SCP: SL server securely and efficiently!

