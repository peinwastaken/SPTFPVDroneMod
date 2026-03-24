using EFT;

namespace FPVDroneModClient.Interface
{
    public interface IOwnable
    {
        public IPlayer Owner { get; set; }
    }
}
