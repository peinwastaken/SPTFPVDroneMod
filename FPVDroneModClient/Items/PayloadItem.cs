#if !UNITY_EDITOR
using System;
using System.Collections.Generic;
using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using FPVDroneModClient.Enum;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Interface;
using WTTClientCommonLib.Attributes;

namespace FPVDroneModClient.Items
{
    public class PayloadItemTemplate(
        float damage,
        float maxDistance,
        float fractureDelta,
        float heavyBleedDelta,
        float lightBleedDelta,
        float staminaBurnRate,
        float instantKillDistance = -1f,
        bool isAntiTank = false
    ) : ItemTemplate
    {
        public readonly float Damage = damage;
        public readonly float MaxDistance = maxDistance;
        public readonly float FractureDelta = fractureDelta;
        public readonly float HeavyBleedDelta = heavyBleedDelta;
        public readonly float LightBleedDelta = lightBleedDelta;
        public readonly float StaminaBurnRate = staminaBurnRate;
        public readonly float InstantKillDistance = instantKillDistance;
        public readonly bool IsAntiTank = isAntiTank;
    }

    [CustomParent("69669ea64847b58fd5393f71", typeof(PayloadItem), typeof(PayloadItemTemplate))]
    public class PayloadItem : SlotToggleableItem<PayloadItem>, IExplosive
    {
        public float Damage { get; set; }
        public bool IsAntiTank { get; set; }
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
            IsAntiTank = template.IsAntiTank;
            
            List<ItemAttributeClass> attributes = Attributes.GetFilteredAttributes(
                EItemAttributeId.Resource,
                EItemAttributeId.Size,
                EItemAttributeId.LimitedDiscard
            );

            attributes.AddRange([
                new(EItemAttributeId.MaxAmmoDamage)
                {
                    Name = "EXPLOSION DAMAGE",
                    Base = () => Damage,
                    StringValue = Damage.ToString,
                    DisplayType = () => EItemAttributeDisplayType.Compact,

                },
                new(EItemAttributeId.ExplosionDistance)
                {
                    Name = "EXPLOSION RADIUS",
                    Base = () => MaxDistance,
                    StringValue = () => $"{(int)MaxDistance} {"meters".Localized()}",
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EItemAttributeId.LightBleedingDelta)
                {
                    Name = "INFLICTS LIGHT BLEED",
                    Base = () => LightBleedDelta,
                    StringValue = () => DeltaToPercent(LightBleedDelta),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EItemAttributeId.HeavyBleedingDelta)
                {
                    Name = "INFLICTS HEAVY BLEED",
                    Base = () => HeavyBleedDelta,
                    StringValue = () => DeltaToPercent(HeavyBleedDelta),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EDamageEffectType.Fracture)
                {
                    Name = "INFLICTS FRACTURE",
                    Base = () => FractureDelta,
                    StringValue = () => DeltaToPercent(FractureDelta),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EItemAttributeId.AmmoPenetrationPower)
                {
                    Name = "IS ANTI TANK",
                    StringValue = () => BoolToString(IsAntiTank),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                }
            ]);
                
            Attributes = attributes;
        }

        private string DeltaToPercent(float delta)
        {
            return $"{delta * 100f}%";
        }

        private string BoolToString(bool value)
        {
            return value ? "Yes" : "No";
        }
    }
}
#endif