using _Scripts._Items;
using UnityEngine;

namespace _Scripts._Player {
    public class WeaponSlotManager : MonoBehaviour {
        public WeaponItem attackingWeapon;
        private PlayerManager playerManager;
        private PlayerInventory playerInventory;

        public WeaponHolderSlot leftHandSlot;
        public WeaponHolderSlot rightHandSlot;
        WeaponHolderSlot backSlot;

        public DamageCollider leftHandDamageCollider;
        public DamageCollider rightHandDamageCollider;

        private Animator animator;
        private QuickSlotsUI quickSlotsUI;
        private InputHandler inputHandler;
        private PlayerStats playerStats;

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            playerInventory = GetComponentInParent<PlayerInventory>();
            playerStats = GetComponentInParent<PlayerStats>();
            inputHandler = GetComponentInParent<InputHandler>();

            animator = GetComponent<Animator>();

            quickSlotsUI = FindObjectOfType<QuickSlotsUI>();

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
                else if ( weaponSlot.isBackSlot )
                {
                    backSlot = weaponSlot;
                }
            }
        }

        public void LoadWeaponOnSlot(WeaponItem weaponItem, bool isLeft)
        {
            if ( isLeft )
            {
                leftHandSlot.currentWeapon = weaponItem;
                leftHandSlot.LoadWeaponModel(weaponItem);
                LoadLeftWeaponDamageCollider();
                quickSlotsUI.UpdateWeaponQuickSlotsUI(true, weaponItem);
                #region Handle Left Weapon Idle Animations

                if ( weaponItem != null )
                {
                    animator.CrossFade(weaponItem.leftHandIdle01, 0.2f);
                }
                else
                {
                    animator.CrossFade("Left Arm Empty", 0.2f);
                }

                #endregion
            }
            else
            {
                if ( inputHandler.twoHandFlag )
                {
                    backSlot.LoadWeaponModel(leftHandSlot.currentWeapon);
                    leftHandSlot.UnloadWeaponAndDestroy();
                    animator.CrossFade(weaponItem.twoHandIdle, 0.2f);
                }
                else
                {
                    #region Handle Right Weapon Idle Animations

                    animator.CrossFade("Both Arms Empty", 0.2f);

                    if ( backSlot != null ) backSlot.UnloadWeaponAndDestroy();
                    else Debug.Log("[Info] No Back Slot!");

                    if ( weaponItem != null )
                    {
                        animator.CrossFade(weaponItem.rightHandIdle01, 0.2f);
                    }
                    else
                    {
                        animator.CrossFade("Right Arm Empty", 0.2f);
                    }

                    #endregion
                }
                rightHandSlot.currentWeapon = weaponItem;
                rightHandSlot.LoadWeaponModel(weaponItem);
                LoadRightWeaponDamageCollider();
                quickSlotsUI.UpdateWeaponQuickSlotsUI(false, weaponItem);
            }
        }

        #region Handle Weapon's Damage Collider

        private void LoadLeftWeaponDamageCollider()
        {
            leftHandDamageCollider = leftHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            leftHandDamageCollider.currentWeaponDamage = playerInventory.leftWeapon.baseDamage;
            // not really clean t.b.h.
            leftHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();

        }

        private void LoadRightWeaponDamageCollider()
        {
            rightHandDamageCollider = rightHandSlot.currentWeaponModel.GetComponentInChildren<DamageCollider>();
            rightHandDamageCollider.currentWeaponDamage = playerInventory.rightWeapon.baseDamage;
            // not really clean t.b.h.
            rightHandDamageCollider.characterManager = GetComponentInParent<CharacterManager>();


        }

        public void OpenDamageCollider()
        {
            if ( playerManager.isUsingRightHand )
                rightHandDamageCollider.EnableDamageCollider();
            else if ( playerManager.isUsingLeftHand )
                leftHandDamageCollider.EnableDamageCollider();
            else
                Debug.LogWarning("[Error] No Weapon defined!");
        }

        public void CloseDamageCollider()
        {
            // close both no matter what (!) 
            // What about 2H attacks?!
            // it will be accessed and the collider is destroyed

            if ( rightHandDamageCollider )
                rightHandDamageCollider.DisableDamageCollider();
            if ( leftHandDamageCollider )
                leftHandDamageCollider.DisableDamageCollider();
        }

        //ToDo:  Please replace me with the stuff above (!)
        public void OpenRightDamageCollider()
        {
            Debug.LogError("[Error] I should not be called anymore (!)");
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void OpenLeftDamageCollider()
        {
            Debug.LogError("[Error] I should not be called anymore (!)");
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void CloseRightDamageCollider()
        {
            Debug.LogError("[Error] I should not be called anymore (!)");
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void CloseLeftDamageCollider()
        {
            leftHandDamageCollider.EnableDamageCollider();
            Debug.LogError("[Error] I should not be called anymore (!)");

        }

        #endregion

        // Todo: think of refactoring
        #region Handle Weapon's Stamina Drain

        public void DrainStaminaLightAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStaminaCost * attackingWeapon.lightAttackMultiplier));
        }

        public void DrainStaminaHeavyAttack()
        {
            playerStats.TakeStaminaDamage(Mathf.RoundToInt(attackingWeapon.baseStaminaCost * attackingWeapon.heavyAttackMultiplier));
        }

        #endregion
    }
}