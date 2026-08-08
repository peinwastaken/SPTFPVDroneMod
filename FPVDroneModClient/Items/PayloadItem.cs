using EFT.InventoryLogic;
using FPVDroneModClient.Components;
using FPVDroneModClient.Helpers;
using FPVDroneModClient.Interface;
using WTTClientCommonLib.Attributes;

namespace FPVDroneModClient.Items
{
    public class PayloadItemTemplate(
        bool isAntiTank = false
    ) : AmmoTemplate
    {
        public readonly bool IsAntiTank = isAntiTank;
    }

    [CustomParent("69669ea64847b58fd5393f71", typeof(PayloadItem), typeof(PayloadItemTemplate))]
    public class PayloadItem : Ammo, ISlotToggleable
    {
        public SlotVisibilityToggler SlotToggleController { get; set; }
        public Item Item { get; set; }
        public bool IsAntiTank { get; set; }

        public PayloadItem(string id, PayloadItemTemplate template) : base(id, template)
        {
            var cachedQualities = template
                .GetCachedReadonlyQualities()
                .GetFilteredAttributes(EItemAttributeId.MaxAmmoDamage);
            var attributes = Attributes.GetFilteredAttributes(
                EItemAttributeId.LimitedDiscard
            );
            
            attributes.AddRange([
                new(EItemAttributeId.ExplosionDistance)
                {
                    Name = "EXPLOSION DISTANCE",
                    Base = () => template.MaxExplosionDistance,
                    StringValue = () => MinMaxMeters((int)template.MinExplosionDistance, (int)template.MaxExplosionDistance),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                },
                new(EItemAttributeId.AmmoPenetrationPower)
                {
                    Name = "IS ANTI TANK",
                    StringValue = () => BoolToString(IsAntiTank),
                    DisplayType = () => EItemAttributeDisplayType.Compact
                }
            ]);
            
            SonicType = SonicBulletSoundPlayer.SonicType.SonicShotgun;
            IsAntiTank = template.IsAntiTank;
            Attributes = [..cachedQualities, ..attributes];
        }
        
        private string BoolToString(bool value)
        {
            return value ? "Yes" : "No";
        }

        private string MinMaxMeters(int min, int max)
        {
            return $"{min} - {max} meters";
        }
    }
}

