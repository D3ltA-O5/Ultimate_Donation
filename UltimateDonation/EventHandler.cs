using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

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

    public void OnPlayerVerified(VerifiedEventArgs ev)
    {
        if (_roleManager.IsDonator(ev.Player.UserId))
        {
            string roleKey = _roleManager.GetDonatorRole(ev.Player.UserId);

            if (_config.DonatorRoles.TryGetValue(roleKey, out var donatorRole))
            {
                ev.Player.RankName = donatorRole.RankName;
                ev.Player.RankColor = donatorRole.RankColor;
            }
        }
    }
}
