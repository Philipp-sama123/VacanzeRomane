using _Scripts._AI;
using _Scripts._Player;
using UnityEngine;

namespace _Scripts {
    public class DamageCharacter : MonoBehaviour {
        [SerializeField] private int damage = 25;

        private void OnTriggerEnter(Collider collision)
        {
            Debug.Log("[Info] Attack registered on:  " + collision.name + " from " + gameObject.name);
            if ( collision.CompareTag($"Player") )
            {
                PlayerStats playerStats = collision.GetComponent<PlayerStats>();

                if ( playerStats != null )
                {
                    // Todo: interface in Stats
                    playerStats.TakeDamage(damage);
                }
            }

            if ( collision.CompareTag($"Enemy") )
            {
                EnemyStats enemyStats = collision.GetComponent<EnemyStats>();

                if ( enemyStats != null )
                {
                    enemyStats.TakeDamage(damage);
                }
            }
        }
    }
}