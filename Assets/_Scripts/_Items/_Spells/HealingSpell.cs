using _Scripts._Player;
using UnityEngine;

namespace _Scripts._Items._Spells {
    [CreateAssetMenu(menuName = "Spells/Healing Spell")]
    public class HealingSpell : SpellItem {
        public int healAmount = 20;

        public override void AttemptToCastSpell(
            PlayerAnimatorManager playerAnimatorManager,
            PlayerStats playerStats,
            WeaponSlotManager weaponSlotManager
        )
        {
            base.AttemptToCastSpell(playerAnimatorManager, playerStats, weaponSlotManager);

            GameObject instantiatedWarmUpSpellFX = Instantiate(spellWarmUpFX, playerAnimatorManager.transform);
            playerAnimatorManager.PlayTargetAnimation(spellAnimation, true);
        }

        public override void SuccessfullyCastSpell(
            PlayerAnimatorManager playerAnimatorManager,
            PlayerStats playerStats,
            CameraHandler cameraHandler,
            WeaponSlotManager weaponSlotManager
        )
        {
            base.SuccessfullyCastSpell(playerAnimatorManager, playerStats, cameraHandler, weaponSlotManager);

            GameObject instantiatedSpellFX = Instantiate(spellCastFX, playerAnimatorManager.transform);
            playerStats.HealPlayer(healAmount);
        }
    }
}