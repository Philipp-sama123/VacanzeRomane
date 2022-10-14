using System;
using _Scripts._Items;
using UnityEngine;

namespace _Scripts {
    public class BlockingCollider : MonoBehaviour {
        public BoxCollider blockingBoxCollider;

        public float blockingPhysicalDamageAbsorption;

        public void Awake()
        {
            blockingBoxCollider = GetComponent<BoxCollider>();
        }

        public void SetColliderDamageAbsorption(WeaponItem weapon)
        {
            if ( weapon != null )
            {
                blockingPhysicalDamageAbsorption = weapon.physicalDamageAbsorption;
            }
        }

        public void EnableBlockingCollider()
        {
            blockingBoxCollider.enabled = true;
        }

        public void DisableBlockingCollider()
        {
            blockingBoxCollider.enabled = false;
        }
    }
}