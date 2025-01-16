using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

/// <summary>
/// Класс, обрабатывающий события, связанные с игроками (например, верификация).
/// </summary>
public class EventHandlers
{
    private readonly RoleManager _roleManager;
    private readonly CooldownManager _cooldownManager;
    private readonly Config _config;

    public EventHandlers(RoleManager roleManager, CooldownManager cooldownManager, Config config)
    {
        _roleManager = roleManager;
        _cooldownManager = cooldownManager;
        _config = config;
    }

    /// <summary>
    /// Событие срабатывает, когда игрок успешно зашёл и прошёл аутентификацию (VerifiedEventArgs).
    /// Если у игрока есть активный донат, назначаем ему RankName и RankColor.
    /// </summary>
    public void OnPlayerVerified(VerifiedEventArgs ev)
    {
        // Проверяем, является ли игрок донатором
        if (_roleManager.IsDonator(ev.Player.UserId))
        {
            // Узнаём, какая у него роль (например, "vip" или "premium")
            string roleKey = _roleManager.GetDonatorRole(ev.Player.UserId);

            // Проверяем, есть ли описание этой роли в конфиге (внутри ultimate_donation.donator_roles)
            if (_config.UltimateDonation.donator_roles.TryGetValue(roleKey, out var donatorRole))
            {
                // Выставляем игроку RankName/RankColor
                ev.Player.RankName = donatorRole.rank_name;
                ev.Player.RankColor = donatorRole.rank_color;
            }
        }
    }
}
