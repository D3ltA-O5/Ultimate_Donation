[README на русском](README.ru.md)

# UltimateDonation

**UltimateDonation** is a plugin for [SCP: Secret Laboratory](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory) built with the [EXILED](https://github.com/Exiled-Team/EXILED) framework.  
The plugin gives donors cool in-game perks **while keeping administrative controls locked down via a flexible permission system**.  
All donor data is stored in *real time* inside the `DonationsData.yml` file (no server restart required).

## 📚 Additional Guides for Players

We have ready-to-paste Discord guides that you can share with your community:

- **[English Donor Guide](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/eng_guide_for_donaters)**  
- **[Русский гайд для донатеров](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/ru_guide_for_donaters)**  

---

## 🎯 Purpose

UltimateDonation is designed to give donors exciting privileges, but **never actual admin power**.  
All donor commands are limited, rate-bound, and fully configurable.  
Server staff manage donations through Remote Admin (RA) commands that respect the EXILED / CedMod permission system.

---

## 🎉 Features

| ✔ | Feature | Notes |
|---|---------|-------|
| ✅ | **Dynamic Donor Storage** | Everything lives in `EXILED/Configs/Ultimate_Donation/DonationsData.yml` and updates instantly. |
| ✅ | **Strict Permission Check** | Admin RA commands use standard permission strings (`donator.addrole`, …). |
| ✅ | **Configurable Roles** | Safe, Euclid, Keter examples already included in **Config.yml**. |
| ✅ | **Custom Prefixes** | Optional per-role coloured [VIP] tags. |
| ✅ | **Global / Individual Freeze** | Pause all donations or a single player’s donation timer. |
| ✅ | **Blacklist & Limits** | For roles, items, and per-round command usage. |
| ✅ | **Translation File** | `donat_translations.yml` holds every message + alias. |
| ✅ | **EXILED & CedMod Friendly** | Works out of the box with both permission providers. |

---

## 🚀 Installation

1. **Download** the latest `UltimateDonation.dll` from the [Releases](https://github.com/D3ltA-O5/Ultimate_Donation/releases) page.  
2. **Drop** the file into `EXILED/Plugins`.  
3. **Restart** the server.  
   - The plugin will create `EXILED/Configs/Ultimate_Donation/` and generate: 
     - `donat_translations.yml` → full message template with aliases  
     - `DonationsData.yml` → **example entries** so you see the format immediately.  
- `[your server port]-config.yml` str `ultimate_donation:` → donation role presets (Safe / Euclid / Keter) 
4. **Tweak** roles in `[your server port]-config.yml`, messages in `donat_translations.yml`.  
5. **Set up permissions** (see next section).

---

## 🔐 Setting Up Admin Permissions

UltimateDonation’s admin commands run only if the RA sender has the matching permission string.  
There are **two common ways** to grant them:

### 1. Editing `permissions.yml` directly (pure EXILED)

Add permission lines into the EXILED permissions file:

    groups:
      senioradmin:
        permissions:
          - donator.*              # full access to all plugin commands
          - player.*               # regular RA commands, example
        inheritance: []
        is_hidden: false

      junioradmin:
        permissions:
          - donator.addrole
          - donator.removerole
          - donator.infoplayer
        inheritance: []
        is_hidden: false

> Reload with `permissions reload` or restart the server.

**Available permission keys**

| Command | Permission |
|---------|------------|
| `donator addrole` | `donator.addrole` |
| `donator removerole` | `donator.removerole` |
| `donator freezeall` | `donator.freezeall` |
| `donator freezeplayer` | `donator.freezeplayer` |
| `donator infoplayer` | `donator.infoplayer` |
| `donator listroleplayers` | `donator.listroleplayers` |
| `donator listalldonations` | `donator.listalldonations` |
| *all of the above* | `donator.*` |

### 2. Via CedMod web panel

CedMod reads the same permission strings, but you can edit them in the browser:

1. Open **CedMod → Groups**.  

2. Click **Create** (or edit an existing group). 

![CedMod permission UI – placeholder](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/README_Resources/Screenshot%202025-05-11%20182346.png) 

3. In *Custom permissions* add lines like `donator.addrole` or `donator.*` or any other permission.  

![CedMod permission UI 2 – placeholder](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/README_Resources/Screenshot%202025-05-11%20182505.png) 

4. Save and assign users to that group.

> The change is live instantly — no file editing required.

<div align="center">

</div>

---

## 📋 Commands

### For Players

| Command | Aliases | Description |
|---------|---------|-------------|
| `.changerole` | `.cr`, `.role`, `.chrole` | Change your current class (if allowed). |
| `.giveitem` | `.gi`, `.givei`, `.giveweapon` | Spawn yourself an item / weapon. |
| `.mydon` | `.mydonation`, `.mydn`, `.md` | See remaining days, limits, permissions. |
| `.donator prefix <Prefix> <Color>` | — | Set a custom tag (if role allows). |

### For Admins (Remote Admin)

| Command | Description |
|---------|-------------|
| `donator addrole <SteamID64> <roleKey> <days>` | Give a donor role. |
| `donator removerole <SteamID64>` | Remove donor role. |
| `donator freezeall <true|false>` | Stop/Resume all donations globally. |
| `donator freezeplayer <SteamID64> <true|false>` | Stop/Resume one player’s timer. |
| `donator infoplayer <SteamID64>` | Detailed info on one donation. |
| `donator listroleplayers <roleKey>` | List everyone with that role. |
| `donator listalldonations` | Dump every donation entry. |

*(Admin commands require the permissions shown above.)*

---

## 📋 Configuration Overview

| File | Purpose |
|------|---------|
| `EXILED/Configs/Ultimate_Donation/Config.yml` | Main plugin config, role presets, limits, blacklist. |
| `EXILED/Configs/Ultimate_Donation/donat_translations.yml` | All messages / aliases. |
| `EXILED/Configs/Ultimate_Donation/DonationsData.yml` | **Dynamic** donor storage (autogenerated with sample data). |

### Role snippet (from Config.yml)

    donator_roles:
      safe:
        name: "Safe"
        badge_color: "green"
        permissions: [ "changerole", "giveitem" ]
        rank_name: "SAFE"
        rank_color: "green"
        customprefixenabled: false
      keter:
        name: "Keter"
        badge_color: "red"
        permissions: [ "changerole", "giveitem" ]
        rank_name: "KETER"
        rank_color: "red"
        customprefixenabled: true

### Example entry in DonationsData.yml

    player_donations:
      - nickname: "ExampleUser"
        steam_id: "76561198000000000"
        role: "safe"
        expiry_date: 2025-01-31
        is_frozen: false

---

## 📋 Translation File Highlights

`donat_translations.yml` contains every player-facing phrase.  
Just change the text — no recompilation needed.

    help_changerole_usage: "Usage: .changerole <RoleAlias>"
    change_role_success: "You changed your role to {roleName}."
    mydon_status_info: |
      === Your Donation Status ===
      - Role: {roleName}
      - Days Left: {daysLeft}
      - Usage: {usageSummary}

Placeholders inside braces (`{roleName}`) are replaced automatically.

---

## 📋 Requirements

- **EXILED** 9.0.0+ (or compatible Nightly).  
- Works with **CedMod** & **MER** out of the box.

---

## ⚠️ Important Notes

1. **Donor ≠ Admin**: even with a Keter badge, donors have *zero* RA access.  
2. **Real-time data**: `DonationsData.yml` is saved immediately after each RA command.  
3. **Debug logs**: set `debug: true` in `Config.yml` to trace everything.  
4. **Wildcard carefully**: giving `donator.*` equals full donation control.

---

## ✅ What’s New (v1.1.0)

- Automatic folder generation `Ultimate_Donation/`.  
- Sample data inside fresh `DonationsData.yml`.  
- Permission strings for each admin command.  
- Cleaner console output (no stray formatting tags).  

*(Full changelog on the [Releases](https://github.com/D3ltA-O5/Ultimate_Donation/releases) page.)*

---

## 📧 Contact & Support

- **GitHub Issues**: <https://github.com/D3ltA-O5/Ultimate_Donation/issues>  
- **Discord**: **cyberco**

Thank you for using **UltimateDonation** — happy managing!
