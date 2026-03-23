namespace FPVDroneModClient.Interface
{
    public interface IPilotable
    {
        public void OnPilotEnter(bool isDoneLocally);

        public void OnPilotExit(bool isDoneLocally);
    }
}
