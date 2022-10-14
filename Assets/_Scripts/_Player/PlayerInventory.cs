using System.Collections.Generic;
using _Scripts._Items;
using _Scripts._Items._Spells;
using UnityEngine;

namespace _Scripts._Player {
    public class PlayerInventory : MonoBehaviour {
        WeaponSlotManager weaponSlotManager;
        public SpellItem currentSpell;

        public WeaponItem rightWeapon;
        public WeaponItem leftWeapon; // ToDo: --> also activate left Weapon
        public WeaponItem unarmedWeapon;

        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1];
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[1];

        public int currentRightWeaponIndex = 0;
        public int currentLeftWeaponIndex = 0;

        public List<WeaponItem> weaponsInventory;

        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
        }

        private void Start()
        {
            rightWeapon = weaponsInRightHandSlots[0];
            leftWeapon = weaponsInLeftHandSlots[0];
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
        }

        public void ChangeRightWeapon()
        {
            WeaponItem weapon = null;
            while ( currentRightWeaponIndex < weaponsInRightHandSlots.Length - 1 && weapon == null )
            {
                currentRightWeaponIndex += 1;
                weapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            }
            if ( weapon != null )
            {
                rightWeapon = weapon;
                weaponSlotManager.LoadWeaponOnSlot(weapon, false);
            }
            else
            {
                LoadRightUnarmedWeapon();
            }
        }

        public void ChangeLeftWeapon()
        {
            WeaponItem weapon = null;
            while ( currentLeftWeaponIndex < weaponsInLeftHandSlots.Length - 1 && weapon == null )
            {
                currentLeftWeaponIndex += 1;
                weapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            }
            if ( weapon != null )
            {
                leftWeapon = weapon;
                weaponSlotManager.LoadWeaponOnSlot(weapon, true);
            }
            else
            {
                LoadLeftUnarmedWeapon();
            }
            // currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            // if ( currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null )
            // {
            //     leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            //     weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            // }
            // else if ( currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] == null )
            // {
            //     currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            // }
            // else if ( currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] == null )
            // {
            //     leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
            //     weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            // }
            // else
            // {
            //     currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            // }
            //
            // if ( currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1 )
            // {
            //     currentLeftWeaponIndex = -1;
            //     leftWeapon = unarmedWeapon;
            //     weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
            // }
        }

        private void LoadLeftUnarmedWeapon()
        {
            currentLeftWeaponIndex = -1;
            leftWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
        }

        private void LoadRightUnarmedWeapon()
        {
            currentRightWeaponIndex = -1;
            rightWeapon = unarmedWeapon;
            weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
        }
    }
}