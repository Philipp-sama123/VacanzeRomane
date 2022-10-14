using _Scripts._Items;
using _Scripts._Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts {
    public class WeaponPickUp : Interactable {

        public WeaponItem weapon;

        public override void Interact(PlayerManager playerManager)
        {
            base.Interact(playerManager);
            //Todo:  Pickup the Item and add it to the Player Inventory
            PickUpItem(playerManager);
        }

        private void PickUpItem(PlayerManager playerManager)
        {
            PlayerInventory playerInventory = playerManager.GetComponent<PlayerInventory>();
            PlayerLocomotion playerLocomotion = playerManager.GetComponent<PlayerLocomotion>();
            PlayerAnimatorManager playerAnimatorManager = playerManager.GetComponentInChildren<PlayerAnimatorManager>();

            playerLocomotion.rigidbody.velocity = Vector3.zero; // stops player from moving while picking up the item 
            playerAnimatorManager.PlayTargetAnimation("[Action] Pick Up Item", true);
            playerInventory.weaponsInventory.Add(weapon);

            playerManager.itemInteractableGameObject.GetComponentInChildren<Text>().text = weapon.itemName;
            playerManager.itemInteractableGameObject.GetComponentInChildren<RawImage>().texture = weapon.itemIcon.texture;
            playerManager.itemInteractableGameObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}