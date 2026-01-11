namespace FPVDroneModClient.Globals
{
    public static class Category
    {
        public static string General = "General";
        public static string Drone = "FPV Drone";
        public static string Binds = "FPV Drone Keybinds";
        public static string ReconDrone = "Recon Drone";
        public static string ReconBinds = "Recon Drone Keybinds";
        public static string PP = "Post Processing";
        public static string Explosion = "Explosions";

        public static string Format(int order, string category)
        {
            return $"{order}. {category}";
        }
    }
}
