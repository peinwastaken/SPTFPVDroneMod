using EFT.InventoryLogic;
using System;
using UnityEngine;

namespace FPVDroneModClient.Interface
{
    public interface IDetonatable
    {
        public event Action OnDetonate;
        public void Detonate();
    }
}
