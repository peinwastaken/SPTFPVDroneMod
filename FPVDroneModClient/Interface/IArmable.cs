using System;

namespace FPVDroneModClient.Interface
{
    public interface IArmable
    {
        bool IsArmed { get; set; }
        event Action<bool> OnToggleArmed;
        void ToggleArmed();
    }
}
