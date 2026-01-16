#if !UNITY_EDITOR
using System;
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

    public class PayloadItem : Item, IExplosive
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
            FractureDelta = template.FractureDelta;
            HeavyBleedDelta = template.HeavyBleedDelta;
            LightBleedDelta = template.LightBleedDelta;
            StaminaBurnRate = template.StaminaBurnRate;
            InstantKillDistance = template.InstantKillDistance;
            
            List<ItemAttributeClass> attributes = [
                new(EPayloadAttribute.Damage)
                {
                    Name = "Explosion Damage",
                    Base = () => Damage,
                    StringValue = Damage.ToString,
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EPayloadAttribute.Range)
                {
                    Name = "Explosion Radius",
                    Base = () => MaxDistance,
                    StringValue = MaxDistance.ToString,
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EPayloadAttribute.LightBleedChance)
                {
                    Name = "Inflicts Light Bleed",
                    Base = () => LightBleedDelta,
                    StringValue = () => DeltaToPercent(LightBleedDelta),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EPayloadAttribute.HeavyBleedChance)
                {
                    Name = "Inflicts Heavy Bleed",
                    Base = () => HeavyBleedDelta,
                    StringValue = () => DeltaToPercent(HeavyBleedDelta),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EPayloadAttribute.FractureChance)
                {
                    Name = "Inflicts Fracture",
                    Base = () => FractureDelta,
                    StringValue = () => DeltaToPercent(FractureDelta),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                }
            ];
                
            Attributes = attributes;
        }

        private string DeltaToPercent(float delta)
        {
            return $"{delta * 100f}%";
        }
    }
}
#endif