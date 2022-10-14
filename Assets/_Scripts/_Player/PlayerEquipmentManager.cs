using System;
using UnityEngine;

namespace _Scripts._Player {
    public class PlayerEquipmentManager : MonoBehaviour {
        public BlockingCollider blockingCollider;
        private PlayerInventory playerInventory;
        private InputHandler inputHandler;

        private void Awake()
        {
            playerInventory = GetComponentInParent<PlayerInventory>();
            inputHandler = GetComponentInParent<InputHandler>();
            if(blockingCollider == null) Debug.LogWarning("[Action Required] Please Assign the blocking collider");
        }

        public void OpenBlockingCollider()
        {
            if ( inputHandler.twoHandFlag )
            {
                blockingCollider.SetColliderDamageAbsorption(playerInventory.rightWeapon);
            }
            else
            {
                blockingCollider.SetColliderDamageAbsorption(playerInventory.leftWeapon);

            }
            blockingCollider.EnableBlockingCollider();
        }

        public void CloseBlockingCollider()
        {
            blockingCollider.DisableBlockingCollider();
        }
    }
}