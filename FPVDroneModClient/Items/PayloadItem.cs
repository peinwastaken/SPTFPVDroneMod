using System.Collections.Generic;
using EFT.InventoryLogic;
using FPVDroneModClient.Enum;
using FPVDroneModClient.Interface;

namespace FPVDroneModClient.Items
{
    public class PayloadItemTemplate(
        float damage,
        float maxDistance,
        float fractureDelta,
        float heavyBleedDelta,
        float lightBleedDelta,
        float staminaBurnRate,
        float instantKillDistance = -1f
    ) : ItemTemplate
    {
        public readonly float Damage = damage;
        public readonly float MaxDistance = maxDistance;
        public readonly float FractureDelta = fractureDelta;
        public readonly float HeavyBleedDelta = heavyBleedDelta;
        public readonly float LightBleedDelta = lightBleedDelta;
        public readonly float StaminaBurnRate = staminaBurnRate;
        public readonly float InstantKillDistance = instantKillDistance;
    }

    public class PayloadItem : Item, IArmable, IExplosive
    {
        public bool IsArmed { get; set; } = false;
        public float Damage { get; set; }
        public float MaxDistance { get; set; }
        public float FractureDelta { get; set; }
        public float HeavyBleedDelta { get; set; }
        public float LightBleedDelta { get; set; }
        public float StaminaBurnRate { get; set; }
        public float InstantKillDistance { get; set; }

        public PayloadItem(string id, PayloadItemTemplate template) : base(id, template)
        {
            Damage = template.Damage;
            MaxDistance = template.MaxDistance;
            
            List<ItemAttributeClass> attributes = [
                new(EPayloadAttribute.Damage)
                {
                    Name = "Damage",
                    Base = () => Damage,
                    StringValue = Damage.ToString,
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EPayloadAttribute.Range)
                {
                    Name = "Range",
                    Base = () => MaxDistance,
                    StringValue = MaxDistance.ToString,
                    DisplayType = () => EItemAttributeDisplayType.Compact
                }
            ];
                
            Attributes = attributes;
        }
    
        public void ToggleArmed()
        {
            IsArmed = !IsArmed;
        }
    }
}