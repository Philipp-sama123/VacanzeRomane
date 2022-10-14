using System;
using _Scripts._AI;
using UnityEngine;

namespace _Scripts._Items._Spells {
    public class SpellDamageCollider : DamageCollider {
        public GameObject impactParticles;
        public GameObject projectileParticles;
        public GameObject muzzleParticles;

        private CharacterStats spellTarget;
        private bool hasCollided = false;
        private Vector3 impactNormal; // used to rotate the impact particles
        public new Rigidbody rigidbody;

        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            projectileParticles = Instantiate(projectileParticles, transform.position, transform.rotation);
            projectileParticles.transform.parent = transform;

            if ( muzzleParticles )
            {
                muzzleParticles = Instantiate(muzzleParticles, transform.position, transform.rotation);
                Destroy(muzzleParticles, 2f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            // the Projectile has its own Layer!!
            if ( !hasCollided )
            {
                spellTarget = collision.transform.GetComponent<CharacterStats>();
                if ( spellTarget != null )
                {
                    spellTarget.TakeDamage(currentWeaponDamage);
                }
                hasCollided = true;
                impactParticles = Instantiate(impactParticles, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal));

                Destroy(projectileParticles);
                Destroy(impactParticles, 5f);
                Destroy(gameObject, 5f);
            }
        }
    }
}