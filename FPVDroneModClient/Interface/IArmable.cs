namespace FPVDroneModClient.Interface
{
    public interface IArmable
    {
        bool IsArmed { get; set; }
        void ToggleArmed();
    }
}
