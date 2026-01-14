using FPVDroneModClient.Models;

namespace FPVDroneModClient.Interface;

public interface IExplosive
{
    public float Damage { get; set; }
    public float MaxDistance { get; set; }
    public float FractureDelta { get; set; }
    public float HeavyBleedDelta { get; set; }
    public float LightBleedDelta { get; set; }
    public float StaminaBurnRate { get; set; }
    public float InstantKillDistance { get; set; }
}