using EFT.InventoryLogic;
using System;
using UnityEngine;

namespace FPVDroneModClient.Components
{
    public class SlotVisibilityToggler : MonoBehaviour
    {
        public Transform EquippedTransform;
        public Transform UnequippedTransform;
        
        public Item Item;

        public void OnEquip()
        {
            EquippedTransform.gameObject.SetActive(true);
            UnequippedTransform.gameObject.SetActive(false);
        }

        public void OnUnequip()
        {
            EquippedTransform.gameObject.SetActive(false);
            UnequippedTransform.gameObject.SetActive(true);
        }
    }
}