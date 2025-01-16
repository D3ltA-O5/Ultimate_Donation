namespace UltimateDonation
{
    public static class DonatorUtils
    {
        /// <summary>
        /// Strips '@steam' suffix from user IDs if present. E.g. "76561199@steam" => "76561199"
        /// </summary>
        public static string CleanSteamId(string input)
        {
            if (input.EndsWith("@steam", System.StringComparison.OrdinalIgnoreCase))
                return input.Replace("@steam", "");
            return input;
        }
    }
}
