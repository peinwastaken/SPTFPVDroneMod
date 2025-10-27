namespace FPVDroneMod.Globals
{
    public static class Category
    {
        public static string General = "General";
        public static string Drone = "Drone";
        public static string Binds = "Keybinds";

        public static string Format(int order, string category) => $"{order}. {category}";
    }
}
