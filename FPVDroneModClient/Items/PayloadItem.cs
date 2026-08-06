using EFT;
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
            Attributes = template.GetCachedReadonlyQualities();
            SonicType = SonicBulletSoundPlayer.SonicType.SonicShotgun;
            IsAntiTank = template.IsAntiTank;
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

