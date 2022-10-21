using UnityEngine;

namespace _Scripts {
    public class CharacterManager : MonoBehaviour {
        [Header("Lock On Transform")]
        public Transform lockOnTransform;
        [Header("Combat Colliders")]
        public CriticalDamageCollider riposteCollider;
        public CriticalDamageCollider backStabCollider;

        [Header("Combat Flags")]
        public bool canBeRiposted; 
        public bool canBeParried; 
        public bool isParrying; 
        public bool isBlocking;

        [Header("Spells")]
        public bool isFiringSpell; 

        // Damage will be inflicted during an Animation Event
        // Used in backstab or riposte animations
        public int pendingCriticalDamage;
        
        
    }
}