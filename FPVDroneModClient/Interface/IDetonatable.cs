using EFT.InventoryLogic;
using FPVDroneModClient.Models;
using System;
using UnityEngine;

namespace FPVDroneModClient.Interface
{
    public interface IDetonatable
    {
        public event Action OnDetonate;
        public ExplosionData Detonate();
    }
}
