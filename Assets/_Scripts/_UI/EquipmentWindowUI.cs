using _Scripts._Player;
using UnityEngine;

namespace _Scripts {
    public class EquipmentWindowUI : MonoBehaviour {
        public bool rightHandSlot01Selected;
        public bool rightHandSlot02Selected;
        public bool leftHandSlot01Selected;
        public bool leftHandSlot02Selected;

        public HandEquipmentSlotUI[] handEquipmentSlotUis;

        public void LoadWeaponOnEquipmentScreen(PlayerInventory playerInventory)
        {
            for ( int i = 0; i < handEquipmentSlotUis.Length; i++ )
            {
                if ( handEquipmentSlotUis[i].rightHandSlot01 )
                {
                    handEquipmentSlotUis[i].AddItem(playerInventory.weaponsInRightHandSlots[0]);
                }
                else if ( handEquipmentSlotUis[i].rightHandSlot02 )
                {
                    handEquipmentSlotUis[i].AddItem(playerInventory.weaponsInRightHandSlots[1]);
                }
                else if ( handEquipmentSlotUis[i].leftHandSlot01 )
                {
                    handEquipmentSlotUis[i].AddItem(playerInventory.weaponsInLeftHandSlots[0]);
                }
                else if ( handEquipmentSlotUis[i].leftHandSlot02 )
                {
                    handEquipmentSlotUis[i].AddItem(playerInventory.weaponsInLeftHandSlots[1]);
                }
            }
        }
        

        public void SelectRightHandSlot01()
        {
            rightHandSlot01Selected = true;
        }

        public void SelectRightHandSlot02()
        {
            rightHandSlot02Selected = true;
        }

        public void SelectLeftHandSlot01()
        {
            leftHandSlot01Selected = true;
        }

        public void SelectLeftHandSlot02()
        {
            leftHandSlot02Selected = true;
        }
    }
}