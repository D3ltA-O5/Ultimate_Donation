using System;

namespace UltimateDonation
{
    public static class DonatorUtils
    {
        public static string CleanSteamId(string input)
        {
            if (input.EndsWith("@steam", StringComparison.OrdinalIgnoreCase))
                return input.Replace("@steam", "");
            return input;
        }
    }
}