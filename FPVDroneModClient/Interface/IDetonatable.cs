using EFT.InventoryLogic;
using UnityEngine;

namespace FPVDroneModClient.Interface
{
    public interface IDetonatable
    {
        public void Detonate(Vector3 position, IPlayerOwner playerOwner, Item weapon);
    }
}
