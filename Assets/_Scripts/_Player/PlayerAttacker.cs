using System;
using _Scripts._Items;
using UnityEngine;

namespace _Scripts._Player {
    public class PlayerAttacker : MonoBehaviour {
        private PlayerEquipmentManager playerEquipmentManager;
        private PlayerAnimatorManager playerAnimatorManager;
        private WeaponSlotManager weaponSlotManager;

        private PlayerInventory playerInventory;
        private PlayerManager playerManager;
        private InputHandler inputHandler;
        private PlayerStats playerStats;

        private CameraHandler cameraHandler;

        private string lastAttack;
        public LayerMask backStabLayer = 1 << 12; // the 12th Layer
        public LayerMask riposteLayer = 1 << 13; // the 13th Layer

        private void Awake()
        {
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            weaponSlotManager = GetComponent<WeaponSlotManager>();

            playerInventory = GetComponentInParent<PlayerInventory>();
            playerManager = GetComponentInParent<PlayerManager>();
            inputHandler = GetComponentInParent<InputHandler>();
            playerStats = GetComponentInParent<PlayerStats>();

            if ( Camera.main != null ) cameraHandler = Camera.main.GetComponentInParent<CameraHandler>();
            else Debug.LogWarning("[Action Required] No main Camera in Scene!");
        }

        /**
        * General ToDo: handle the second Hand ! 
        */
        public void HandleRbInput()
        {
            if ( playerInventory.rightWeapon.isMeleeWeapon )
            {
                PerformLightAttackMeleeAction();
            }
            else if (
                playerInventory.rightWeapon.isSpellCaster
                || playerInventory.rightWeapon.isFaithCaster
                || playerInventory.rightWeapon.isPyroCaster
            )
            {
                PerformRbMagicAction(playerInventory.rightWeapon);
            }
            else
            {
                Debug.LogWarning("[Action Required] Please assign a WeaponType for your current weapon in the inspector");
            }
        }

        public void HandleRtInput(WeaponItem weapon)
        {
            if ( playerStats.currentStamina <= 0 ) return;

            weaponSlotManager.attackingWeapon = weapon;

            if ( inputHandler.twoHandFlag )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.thHeavyAttack01, true, true);
                lastAttack = weapon.thHeavyAttack01;
            }
            else if ( playerInventory.rightWeapon.isMeleeWeapon || playerInventory.rightWeapon.isUnarmed )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohHeavyAttack01, true, true);
                lastAttack = weapon.ohHeavyAttack01;
            }
            else if (
                playerInventory.rightWeapon.isSpellCaster
                || playerInventory.rightWeapon.isFaithCaster
                || playerInventory.rightWeapon.isPyroCaster
            )
            {
                // Handle Magic Spell casting
                // TODO: Handle Heavy Magic Input
                PerformRbMagicAction(playerInventory.rightWeapon);
            }
        }

        private void PerformLightAttackMeleeAction()
        {
            if ( playerManager.canDoCombo )
            {
                inputHandler.comboFlag = true;
                playerAnimatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
                HandleWeaponCombo(playerInventory.rightWeapon);
                inputHandler.comboFlag = false;
            }
            else
            {
                if ( playerManager.isInteracting || playerManager.canDoCombo ) return;

                playerAnimatorManager.animator.SetBool("IsUsingRightHand", true); // Todo: think of removing it here - but where (?) -oo-> Animator handler + appropriate fct. 
                HandleLightAttack(playerInventory.rightWeapon);
            }
        }

        private void PerformRbMagicAction(WeaponItem weapon)
        {
            if ( playerManager.isInteracting ) return;

            if ( weapon.isFaithCaster )
            {
                if ( playerInventory.currentSpell != null && playerInventory.currentSpell.isFaithSpell )
                {
                    if ( playerStats.currentMana >= playerInventory.currentSpell.manaCost )
                    {
                        playerInventory.currentSpell.AttemptToCastSpell(playerAnimatorManager, playerStats, weaponSlotManager);
                    }
                    else
                    {
                        playerAnimatorManager.PlayTargetAnimation("[Magic] No Mana", true);
                    }
                }
            }
            if ( weapon.isPyroCaster )
            {
                if ( playerInventory.currentSpell != null && playerInventory.currentSpell.isPyroSpell )
                {
                    if ( playerStats.currentMana >= playerInventory.currentSpell.manaCost )
                    {
                        playerInventory.currentSpell.AttemptToCastSpell(playerAnimatorManager, playerStats, weaponSlotManager);
                    }
                    else
                    {
                        playerAnimatorManager.PlayTargetAnimation("[Magic] No Mana", true);
                    }
                }
            }
        }

        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if ( playerStats.currentStamina <= 0 ) return;

            if ( inputHandler.comboFlag )
            {
                playerAnimatorManager.animator.SetBool("CanDoCombo", false);

                HandleOneHandedAttackCombos(weapon);

                if ( lastAttack == weapon.thLightAttack01 )
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.thLightAttack02, true, true);
                    lastAttack = weapon.thLightAttack02;
                }
                else if ( lastAttack == weapon.thLightAttack02 )
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.thLightAttack02, true, true);
                    //todo: lastAttack = weapon.thLightAttack03;
                }

                if ( lastAttack == weapon.thHeavyAttack01 )
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.thHeavyAttack02, true, true);
                    lastAttack = weapon.thHeavyAttack02;
                }
                else if ( lastAttack == weapon.thHeavyAttack01 )
                {
                    playerAnimatorManager.PlayTargetAnimation(weapon.thHeavyAttack02, true, true);
                    //ToDo: lastAttack = weapon.thHeavyAttack03;
                }
            }
        }

        private void HandleOneHandedAttackCombos(WeaponItem weapon)
        {
            if ( lastAttack == weapon.ohLightAttack01 )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohLightAttack02, true, true);
                lastAttack = weapon.ohLightAttack02;
            }
            else if ( lastAttack == weapon.ohLightAttack02 )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohLightAttack03, true, true);
                lastAttack = weapon.ohLightAttack03;
            }
            else if ( lastAttack == weapon.ohLightAttack03 )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohLightAttack04, true, true);
                lastAttack = weapon.ohLightAttack04;
            }


            if ( lastAttack == weapon.ohHeavyAttack01 )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohHeavyAttack02, true, true);
                lastAttack = weapon.ohHeavyAttack02;
            }
            else if ( lastAttack == weapon.ohHeavyAttack02 )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohHeavyAttack03, true, true);
                lastAttack = weapon.ohHeavyAttack03;
            }
            else if ( lastAttack == weapon.ohHeavyAttack03 )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohHeavyAttack04, true, true);
                lastAttack = weapon.ohHeavyAttack04;
            }

        }

        private void HandleLightAttack(WeaponItem weapon)
        {
            if ( playerStats.currentStamina <= 0 ) return;

            weaponSlotManager.attackingWeapon = weapon;

            if ( inputHandler.twoHandFlag )
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.thLightAttack01, true, true);
                lastAttack = weapon.thLightAttack01;
            }
            else
            {
                playerAnimatorManager.PlayTargetAnimation(weapon.ohLightAttack01, true, true);
                lastAttack = weapon.ohLightAttack01;
            }
        }

        public void HandleLbAction()
        {
            PerformBlockingAction();
        }

        public void HandleLtAction()
        {
            if ( playerInventory.leftWeapon.isShieldWeapon )
            {
                PerformParryWeaponArt(inputHandler.twoHandFlag);
            }
            else if ( playerInventory.leftWeapon.isMeleeWeapon )
            {
                // do a light attack
            }
        }

        private void PerformParryWeaponArt(bool isTwoHanding)
        {
            if ( playerManager.isInteracting ) return;

            if ( isTwoHanding )
            {
                // two handing weapon art
            }
            else
            {
                playerAnimatorManager.PlayTargetAnimation(playerInventory.leftWeapon.weaponArt, true);
            }
        }

        public void AttemptBackStabOrRiposte()
        {
            if ( playerStats.currentStamina <= 0 ) return; // ToDo: challenge this 

            RaycastHit hit;

            if ( Physics.Raycast(
                    inputHandler.criticalAttackRaycastStartPoint.position,
                    transform.TransformDirection(Vector3.forward),
                    out hit, 0.5f, backStabLayer
                ) )
            {
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = weaponSlotManager.rightHandDamageCollider;

                if ( enemyCharacterManager != null )
                {
                    playerManager.transform.position = Vector3.Lerp(
                        playerManager.transform.position,
                        enemyCharacterManager.backStabCollider.criticalDamageStandPosition.position,
                        Time.deltaTime / 0.1f
                    ); // ToDo: whats better ... OR: playerManager.transform.position = enemyCharacterManager.backStabCollider.backStabberStandPoint.position; 

                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();

                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    int criticalDamage = playerInventory.rightWeapon.criticalDamageMultiplier * rightWeapon.currentWeaponDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    playerAnimatorManager.PlayTargetAnimation("[BackStab] Stab", true);
                    enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("[BackStab] Stabbed", true);
                    // Check for TeamID (so you cant backstab friends or yourself) 
                    // Pull it into a transform behind the enemy so the backstab looks clean
                    // rotate towards that transform 
                    // play animation 
                    // make enemy play animation 
                    // do damage
                }
            }
            else if ( Physics.Raycast(
                         inputHandler.criticalAttackRaycastStartPoint.position,
                         transform.TransformDirection(Vector3.forward),
                         out hit, 0.75f, riposteLayer
                     ) )
            {
                // check for team I.D.
                CharacterManager enemyCharacterManager = hit.transform.gameObject.GetComponentInParent<CharacterManager>();
                DamageCollider rightWeapon = weaponSlotManager.rightHandDamageCollider;

                if ( playerManager != null && enemyCharacterManager.canBeRiposted )
                {
                    playerManager.transform.position = enemyCharacterManager.riposteCollider.criticalDamageStandPosition.position;

                    Vector3 rotationDirection = playerManager.transform.root.eulerAngles;
                    rotationDirection = hit.transform.position - playerManager.transform.position;
                    rotationDirection.y = 0;
                    rotationDirection.Normalize();
                    Quaternion tr = Quaternion.LookRotation(rotationDirection);
                    Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, 500 * Time.deltaTime);
                    playerManager.transform.rotation = targetRotation;

                    int criticalDamage = playerInventory.rightWeapon.criticalDamageMultiplier * rightWeapon.currentWeaponDamage;
                    enemyCharacterManager.pendingCriticalDamage = criticalDamage;

                    playerAnimatorManager.PlayTargetAnimation("[Combat Action] Riposte", true);
                    enemyCharacterManager.GetComponentInChildren<AnimatorManager>().PlayTargetAnimation("[Combat Action] Riposted", true);
                }
                else
                {
                    // ToDo: when new riposting input -- Animation for no riposte possible
                }

            }
        }

        private void PerformBlockingAction()
        {
            if ( playerManager.isInteracting )
                return;
            if ( playerManager.isBlocking )
                return;

            playerAnimatorManager.PlayTargetAnimation("[Combat Action] Blocking Start", false, true);
            playerEquipmentManager.OpenBlockingCollider();
            playerManager.isBlocking = true;
        }

        private void SuccessfullyCastSpell() // called on Animator Event
        {
            playerInventory.currentSpell.SuccessfullyCastSpell(playerAnimatorManager, playerStats, cameraHandler, weaponSlotManager);
            playerAnimatorManager.animator.SetBool("IsFiringSpell", true);
        }
    }

}