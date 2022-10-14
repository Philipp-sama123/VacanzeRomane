using _Scripts._Player;
using UnityEngine;

namespace _Scripts._Items._Spells {
    public class SpellItem : Item {
        public GameObject spellWarmUpFX;
        public GameObject spellCastFX;

        public string spellAnimation;

        [Header("Spell Cost")]
        public int manaCost;

        [Header("Spell Type")] // ToDo: Think of Enum when it gets bigger
        public bool isFaithSpell;
        public bool isMagicSpell;
        public bool isPyroSpell;

        [Header("Spell description")]
        [TextArea]
        public string spellDescription;

        public virtual void AttemptToCastSpell(
            PlayerAnimatorManager playerAnimatorManager,
            PlayerStats playerStats,
            WeaponSlotManager weaponSlotManager
        )
        {
            Debug.Log("[Info] You attempt to cast a Spell!");
        }

        public virtual void SuccessfullyCastSpell(
            PlayerAnimatorManager playerAnimatorManager,
            PlayerStats playerStats,
            CameraHandler cameraHandler,
            WeaponSlotManager weaponSlotManager
        )
        {
            Debug.Log("[Info] You successfully cast a Spell!");
            playerStats.ReduceMana(manaCost);
        }

    }
}