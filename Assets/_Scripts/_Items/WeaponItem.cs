using UnityEngine;

namespace _Scripts._Items {
    [CreateAssetMenu(menuName = "Items/Weapon Item")]
    public class WeaponItem : Item {
        public GameObject modelPrefab;
        public bool isUnarmed;
        [Header("Damage")]
        public int baseDamage = 25;
        public int criticalDamageMultiplier = 5;

        [Header("Absorption")]
        public float physicalDamageAbsorption = 10f; 
        
        [Header("Idle Animations")]
        public string rightHandIdle01;
        public string leftHandIdle01;
        public string twoHandIdle;

        [Header("One Handed Attack Animations")]
        public string ohLightAttack01;
        public string ohLightAttack02;
        public string ohLightAttack03;
        public string ohLightAttack04;
        
        public string ohHeavyAttack01;
        public string ohHeavyAttack02;
        public string ohHeavyAttack03;
        public string ohHeavyAttack04;

        [Header("Two Handed Attack Animations")]
        public string thLightAttack01;
        public string thLightAttack02;
        public string thHeavyAttack01;
        public string thHeavyAttack02;

        [Header("Weapon Art -- Special Abilities")]
        public string weaponArt;

        [Header("Stamina Costs")]
        public int baseStaminaCost;
        public float lightAttackMultiplier;
        public float heavyAttackMultiplier;

        [Header("Weapon Type")]
        public bool isMeleeWeapon;
        public bool isShieldWeapon;

        public bool isSpellCaster;
        public bool isFaithCaster;
        public bool isPyroCaster;
    }
}