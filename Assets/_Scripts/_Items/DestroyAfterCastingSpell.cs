using UnityEngine;

namespace _Scripts._Items {
    public class DestroyAfterCastingSpell : MonoBehaviour {
        private CharacterManager characterCastingSpell;

        private void Awake()
        {
            characterCastingSpell = GetComponentInParent<CharacterManager>();
            if ( characterCastingSpell == null )
            {
                Debug.LogWarning("[Action Required] Character could not be found!");
            }
        }

        private void Update()
        {
            if ( characterCastingSpell != null && characterCastingSpell.isFiringSpell )
            {
                Destroy(gameObject);
            }
        }
    }
}