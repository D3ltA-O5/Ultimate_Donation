[README in English](README.md)

# UltimateDonation

**UltimateDonation** — это плагин для [SCP: Secret Laboratory](https://store.steampowered.com/app/700330/SCP_Secret_Laboratory), написанный на базе фреймворка [EXILED](https://github.com/Exiled-Team/EXILED).  
Он даёт донат-игрокам приятные игровые бонусы, **оставляя админ-управление только тем, у кого есть соответствующие права**.  
Все данные о донатах хранятся *в реальном времени* в файле `DonationsData.yml` (перезапуск сервера не нужен).

## 📚 Дополнительные гайды для игроков

У нас есть готовые Discord-гайды, которые можно сразу отправить комьюнити:

- **[English Donor Guide](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/eng_guide_for_donaters)**  
- **[Русский гайд для донатеров](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/ru_guide_for_donaters)**  

---

## 🎯 Назначение

UltimateDonation создан, чтобы дать донатерам плюшки, но **никогда не давать им админ-силу**.  
Все игроковые команды ограничены, имеют лимиты и полностью настраиваются.  
Персонал сервера управляет донатами через Remote Admin (RA), а права проверяются через EXILED / CedMod.

---

## 🎉 Возможности

| ✔ | Функция | Что это значит |
|---|---------|----------------|
| ✅ | **Динамичное хранилище** | Всё лежит в `EXILED/Configs/Ultimate_Donation/DonationsData.yml` и обновляется мгновенно. |
| ✅ | **Чёткая проверка прав** | RA-команды используют строки `donator.addrole`, `donator.*` и т.д. |
| ✅ | **Примеры ролей** | Safe, Euclid, Keter уже прописаны в `Config.yml`. |
| ✅ | **Кастомные префиксы** | Для ролей, где разрешено, можно поставить цветные теги. |
| ✅ | **Глобальная / точечная «заморозка»** | Ставьте на паузу все донаты или только одного игрока. |
| ✅ | **Чёрные списки и лимиты** | На роли, предметы и количество команд за раунд. |
| ✅ | **Файл переводов** | В `donat_translations.yml` можно править любое сообщение и алиасы. |
| ✅ | **Дружит с EXILED и CedMod** | Работает «из коробки» и там, и там. |

---

## 🚀 Установка

1. **Скачайте** последнюю версию `UltimateDonation.dll` с вкладки [Releases](https://github.com/D3ltA-O5/Ultimate_Donation/releases).  
2. **Положите** файл в `EXILED/Plugins`.  
3. **Перезапустите** сервер.  
   - Плагин создаст папку `EXILED/Configs/Ultimate_Donation/` и сгенерирует:  
     - `donat_translations.yml` — шаблон всех сообщений + алиасов  
     - `DonationsData.yml` — **примерные записи**, чтобы было понятно, как выглядит структура  
   - В основном конфиге (`[порт]-config.yml`) появится секция `ultimate_donation:` с ролями Safe / Euclid / Keter  
4. **Настройте** роли в `[порт]-config.yml`, тексты — в `donat_translations.yml`.  
5. **Выдайте права** (см. раздел ниже).

---

## 🔐 Настройка админ-прав

RA-команды плагина выполняются **только** если у отправителя есть нужная строка права.  
Есть **два популярных способа** выдать эти права:

### 1. Прямое редактирование `permissions.yml` (чистый EXILED)

Добавьте права в файл EXILED-разрешений:

```yaml
groups:
  senioradmin:
    permissions:
      - donator.*              # полный доступ к командам плагина
      - player.*               # обычные RA-команды, пример
    inheritance: []
    is_hidden: false

  junioradmin:
    permissions:
      - donator.addrole
      - donator.removerole
      - donator.infoplayer
    inheritance: []
    is_hidden: false
```

> Примените `permissions reload` или просто перезапустите сервер, чтобы обновить права.

### Доступные строки прав

| Команда | Строка права |
|---------|--------------|
| `donator addrole` | `donator.addrole` |
| `donator removerole` | `donator.removerole` |
| `donator freezeall` | `donator.freezeall` |
| `donator freezeplayer` | `donator.freezeplayer` |
| `donator infoplayer` | `donator.infoplayer` |
| `donator listroleplayers` | `donator.listroleplayers` |
| `donator listalldonations` | `donator.listalldonations` |
| **все выше** | `donator.*` |

### 2. Через веб-панель CedMod

CedMod читает те же строки, но их можно задавать в браузере:

1. Откройте **CedMod → Groups**.  
2. Нажмите **Create** или выберите группу для редактирования.  
3. В поле **Custom permissions** впишите `donator.addrole`, `donator.*` или любые нужные строки.  

   ![CedMod permission UI – placeholder](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/README_Resources/Screenshot%202025-05-11%20182346.png)

4. Сохраните и привяжите пользователей к группе.

   ![CedMod permission UI 2 – placeholder](https://github.com/D3ltA-O5/Ultimate_Donation/blob/main/README_Resources/Screenshot%202025-05-11%20182505.png)

> Изменения активируются сразу — без ручного редактирования файлов.

---

## 📋 Команды

### Для игроков

| Команда | Алиасы | Описание |
|---------|--------|----------|
| `.changerole` | `.cr`, `.role`, `.chrole` | Сменить текущий класс (если разрешено). |
| `.giveitem` | `.gi`, `.givei`, `.giveweapon` | Выдать себе предмет / оружие. |
| `.mydon` | `.mydonation`, `.mydn`, `.md` | Показать оставшиеся дни, лимиты, права. |
| `.donator prefix <Префикс> <Цвет>` | — | Задать кастомный тег (если роль позволяет). |

### Для администраторов (Remote Admin)

| Команда | Описание |
|---------|----------|
| `donator addrole <SteamID64> <roleKey> <days>` | Выдать донат-роль. |
| `donator removerole <SteamID64>` | Удалить донат. |
| `donator freezeall <true|false>` | Остановить / возобновить все донаты. |
| `donator freezeplayer <SteamID64> <true|false>` | Заморозить / разморозить счётчик конкретного игрока. |
| `donator infoplayer <SteamID64>` | Подробная информация по донату. |
| `donator listroleplayers <roleKey>` | Список всех с этой ролью. |
| `donator listalldonations` | Вывести все записи донатов. |

*(Для работы команд у RA-пользователя должны быть права из таблицы выше.)*

---

## 📋 Обзор конфигураций

| Файл | Назначение |
|------|-----------|
| `EXILED/Configs/Ultimate_Donation/Config.yml` | Главный конфиг плагина: роли, лимиты, чёрные списки. |
| `EXILED/Configs/Ultimate_Donation/donat_translations.yml` | Все сообщения и алиасы. |
| `EXILED/Configs/Ultimate_Donation/DonationsData.yml` | **Динамическое** хранилище донатов (создаётся с примерами). |

### Фрагмент `donator_roles` (из Config.yml)

```yaml
donator_roles:
  safe:
    name: "Safe"
    badge_color: "green"
    permissions: [ "changerole", "giveitem" ]
    rank_name: "SAFE"
    rank_color: "green"
    customprefixenabled: false

  euclid:
    name: "Euclid"
    badge_color: "orange"
    permissions: [ "changerole", "giveitem" ]
    rank_name: "EUCLID"
    rank_color: "orange"
    customprefixenabled: false

  keter:
    name: "Keter"
    badge_color: "red"
    permissions: [ "changerole", "giveitem" ]
    rank_name: "KETER"
    rank_color: "red"
    customprefixenabled: true
```

### Пример записи в DonationsData.yml

```yaml
player_donations:
  - nickname: "ExampleUser"
    steam_id: "76561198000000000"
    role: "safe"
    expiry_date: 2025-01-31
    is_frozen: false
```

---

## 📋 Файл переводов — быстрый взгляд

`donat_translations.yml` содержит каждую фразу, которую видит игрок.  
Меняйте текст без перекомпиляции плагина.

```yaml
help_changerole_usage: "Использование: .changerole <RoleAlias>"
change_role_success: "Вы сменили роль на {roleName}."
mydon_status_info: |
  === Статус вашего доната ===
  - Роль: {roleName}
  - Осталось дней: {daysLeft}
  - Лимиты: {usageSummary}
```

---

## 📋 Требования

- **EXILED** 9.0.0+ (или nightly-сборка).  
- Полностью совместимо с **CedMod** и **MER**.

---

## ⚠️ Важные замечания

1. **Донат ≠ Админ** — даже обладатель Keter не имеет доступа к RA.  
2. **Мгновенное сохранение** — `DonationsData.yml` обновляется сразу после RA-команды.  
3. **Отладка** — установите `debug: true` в Config.yml, чтобы видеть подробные логи.  
4. **Будьте осторожны с wildcard** — право `donator.*` даёт полный контроль над донатами.

---

## ✅ Что нового (v1.1.0)

- Автосоздание папки `Ultimate_Donation/`.  
- Примерные данные в свежем `DonationsData.yml`.  
- Строки прав для каждой RA-команды.  
- Чистый вывод в консоль без «ломаных» тегов.

*(Полный список изменений — на странице [Releases](https://github.com/D3ltA-O5/Ultimate_Donation/releases).)*

---

## 📧 Контакты и поддержка

- **GitHub Issues**: <https://github.com/D3ltA-O5/Ultimate_Donation/issues>  
- **Discord**: **cyberco**

Спасибо, что используете **UltimateDonation** — приятного администрирования!
