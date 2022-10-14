using _Scripts._Items;
using UnityEngine;

namespace _Scripts {
    public class EnemyWeaponSlotManager : MonoBehaviour {
        public WeaponItem rightHandWeapon;
        public WeaponItem leftHandWeapon;

        private WeaponHolderSlot rightHandSlot;
        private WeaponHolderSlot leftHandSlot;

        private DamageCollider leftHandDamageCollider;
        private DamageCollider rightHandDamageCollider;

        private void Awake()
        {
            WeaponHolderSlot[] weaponHolderSlots = GetComponentsInChildren<WeaponHolderSlot>();
            foreach ( WeaponHolderSlot weaponSlot in weaponHolderSlots )
            {
                if ( weaponSlot.isLeftHandSlot )
                {
                    leftHandSlot = weaponSlot;
                }
                else if ( weaponSlot.isRightHandSlot )
                {
                    rightHandSlot = weaponSlot;
                }
            }
        }

        private void Start()
        {
            LoadWeaponsOnBothHands();
        }

        private void LoadWeaponsOnBothHands()
        {

            if ( rightHandWeapon != null )
            {
                LoadWeaponOnSlot(rightHandWeapon, false);
            }
            else if ( leftHandWeapon != null )
            {
                LoadWeaponOnSlot(leftHandWeapon, true);
            }
            else
            {
                Debug.LogWarning("[Warn] No Weapon defined!");
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weapon, bool isLeft)
        {
            if ( isLeft )
            {
                leftHandSlot.currentWeapon = weapon;
                leftHandSlot.LoadWeaponModel(weapon);
                LoadWeaponsDamageCollider(true);
            }
            else
            {
                rightHandSlot.currentWeapon = weapon;
                rightHandSlot.LoadWeaponModel(weapon);
                LoadWeaponsDamageCollider(false);
            }
        }

        public void LoadWeaponsDamageCollider(bool isLeft)
        {
            if ( isLeft )
            {
                leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
                leftHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();
            }
            else
            {
                rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
                rightHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();

            }
        }

        public void OpenDamageCollider()
        {
            if ( rightHandDamageCollider )
            {
                rightHandDamageCollider.EnableDamageCollider();
            }
            else
                Debug.LogWarning("[Error] No Weapon defined!");
        }

        public void CloseDamageCollider()
        {
            if ( rightHandDamageCollider )
                rightHandDamageCollider.DisableDamageCollider();
            else
                Debug.LogWarning("[Error] No Weapon defined!");

        }

        public void EnableCombo()
        {
            Debug.Log("[ToDo:] Combos!");
        }

        public void DisableCombo()
        {
            Debug.Log("[ToDo:] Combos!");
        }

        public void DrainStaminaLightAttack()
        {
            Debug.Log("[ToDo:] Stamina System!");
        }

        public void DrainStaminaHeavyAttack()
        {
            Debug.Log("[ToDo:] Stamina System!");
        }
    }
}