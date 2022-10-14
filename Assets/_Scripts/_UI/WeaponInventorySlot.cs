using System;
using _Scripts._Items;
using _Scripts._Player;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public class WeaponInventorySlot : MonoBehaviour {
        private WeaponSlotManager weaponSlotManager;
        private PlayerInventory playerInventory;
        private UIManager uiManager;

        public Image icon;
        private WeaponItem item;

        private void Awake()
        {
            weaponSlotManager = FindObjectOfType<WeaponSlotManager>();
            playerInventory = FindObjectOfType<PlayerInventory>();
            uiManager = FindObjectOfType<UIManager>();

        }

        public void AddItem(WeaponItem newItem)
        {
            item = newItem;
            icon.sprite = item.itemIcon;
            icon.enabled = true;
            gameObject.SetActive(true);
        }

        public void ClearInventorySlot()
        {
            item = null;
            icon.sprite = null;
            icon.enabled = false;
            gameObject.SetActive(false);
        }

        public void EquipThisItem()
        {
            if ( uiManager.rightHandSlot01Selected )
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[0]);
                playerInventory.weaponsInRightHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if ( uiManager.rightHandSlot02Selected )
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInRightHandSlots[1]);
                playerInventory.weaponsInRightHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if ( uiManager.leftHandSlot01Selected )
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[0]);
                playerInventory.weaponsInLeftHandSlots[0] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else if ( uiManager.leftHandSlot02Selected )
            {
                playerInventory.weaponsInventory.Add(playerInventory.weaponsInLeftHandSlots[1]);
                playerInventory.weaponsInLeftHandSlots[1] = item;
                playerInventory.weaponsInventory.Remove(item);
            }
            else
            {
                return;
            }

            if ( playerInventory.currentRightWeaponIndex != -1 )
                playerInventory.rightWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentRightWeaponIndex];
            else Debug.LogWarning("Please equip a Weapon - or handle this case!");

            if ( playerInventory.currentRightWeaponIndex != -1 )
                playerInventory.leftWeapon = playerInventory.weaponsInRightHandSlots[playerInventory.currentLeftWeaponIndex];
            else Debug.LogWarning("Please equip a Weapon - or handle this case!");
            
            // reloads Weapons in the Hand for the player
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(playerInventory.leftWeapon, true);

            // update UI View
            uiManager.equipmentWindowUI.LoadWeaponOnEquipmentScreen(playerInventory);
            uiManager.ResetAllSelectedSlots();
        }

    }
}