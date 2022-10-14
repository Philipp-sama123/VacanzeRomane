using _Scripts._Player;
using UnityEngine;

namespace _Scripts._Items._Spells {
    [CreateAssetMenu(menuName = "Spells/Projectile Spell")]
    public class ProjectileSpell : SpellItem {

        [Header("Projectile Damage")]
        public float baseDamage = 25;

        [Header("Projectile Physics")]
        public float projectileForwardVelocity = 500f;
        public float projectileUpwardVelocity = 10f;
        public float projectileMass = 25f;
        public bool isEffectedByGravity = false;

        private Rigidbody rigidbody;


        public override void AttemptToCastSpell(
            PlayerAnimatorManager playerAnimatorManager,
            PlayerStats playerStats,
            WeaponSlotManager weaponSlotManager
        )
        {
            base.AttemptToCastSpell(playerAnimatorManager, playerStats, weaponSlotManager);

            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, weaponSlotManager.rightHandSlot.transform);
            instantiatedWarmUpSpellFX.gameObject.transform.localScale = new Vector3(100, 100, 100);
            playerAnimatorManager.PlayTargetAnimation(spellAnimation, true);

            // Instantiate the Spell 
            // Play animation to cast the spell
        }

        public override void SuccessfullyCastSpell(
            PlayerAnimatorManager playerAnimatorManager,
            PlayerStats playerStats,
            CameraHandler cameraHandler,
            WeaponSlotManager weaponSlotManager
        )
        {
            base.SuccessfullyCastSpell(playerAnimatorManager, playerStats, cameraHandler, weaponSlotManager);
            // beginning right hand only 
            GameObject instantiatedSpellFX = Instantiate(spellCastFX,
                weaponSlotManager.rightHandSlot.transform.position,
                cameraHandler.cameraPivotTransform.rotation);

            rigidbody = instantiatedSpellFX.GetComponent<Rigidbody>();

            if ( cameraHandler.currentLockOnTarget != null )
            {
                instantiatedSpellFX.transform.LookAt(cameraHandler.currentLockOnTarget.transform);
            }
            else
            {
                instantiatedSpellFX.transform.rotation = Quaternion.Euler(cameraHandler.cameraPivotTransform.eulerAngles.x, playerStats.transform.eulerAngles.y, 0);
            }

            rigidbody.AddForce(instantiatedSpellFX.transform.forward * projectileForwardVelocity);
            rigidbody.AddForce(instantiatedSpellFX.transform.up * projectileUpwardVelocity);
            rigidbody.useGravity = isEffectedByGravity;
            rigidbody.mass = projectileMass;
            instantiatedSpellFX.transform.parent = null;
        }
    }
}