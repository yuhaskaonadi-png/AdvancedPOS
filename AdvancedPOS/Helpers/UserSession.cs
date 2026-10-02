namespace AdvancedPOS.Helpers
{
    public static class UserSession
    {
        public static int CurrentUserID { get; set; }
        public static string CurrentUsername { get; set; }
        public static string CurrentRole { get; set; }
    }
}
