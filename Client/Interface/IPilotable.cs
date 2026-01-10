using FPVDroneMod.Components;

namespace FPVDroneMod.Interface
{
    public interface IPilotable
    {
        public void OnPilotEnter();

        public void OnPilotExit();
    }
}
