using System;
using _Scripts._Player;
using UnityEngine;

namespace _Scripts {
    public class Interactable : MonoBehaviour {
        public float radius = 0.5f;
        public string interactableText;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, radius);
        }

        public virtual void Interact(PlayerManager playerManager)
        {
            // Debug.Log("You interacted with an Object");
            // Called when Player Interactss
        }
    }
}