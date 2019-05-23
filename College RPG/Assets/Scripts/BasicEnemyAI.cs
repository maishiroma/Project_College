/*  This simple script just makes the enemy follow the player when they are in their line of sight
 * 
 */

using UnityEngine;
using UnityEngine.AI;

namespace MattScripts {

    // Lists all of the states an enemy can have
    public enum EnemyState {
        PATROL,
        CHASE,
        WAIT,
    }

    public class BasicEnemyAI : MonoBehaviour {

        [Header("General Variables")]
        public EnemyState currentState = EnemyState.WAIT;
        [Range(0.1f,3f)]
        public float timeToReset = 1f;

        [Header("External Variables")]
        public NavMeshAgent enemyAgent;
        public BattleEvent battleEvent;
        public GameObject homeSpawn;

		// Depending on the state of the enemy, we do additional logic checks
		private void Update()
		{
            switch(currentState)
            {
                case EnemyState.PATROL:
                case EnemyState.CHASE:
                    // When we are not in control of our character, the enemy stops
                    if(battleEvent.HasActivated == true || GameManager.Instance.CurrentState != GameStates.NORMAL)
                    {
                        enemyAgent.isStopped = true;
                    }
                    else
                    {
                        enemyAgent.isStopped = false;
                    }
                    break;
            }
		}

		// When the player is in the enemy's range, the enemy will chase them
		private void OnTriggerEnter(Collider other)
		{
            if(other.CompareTag("Player") && currentState == EnemyState.PATROL)
            {
                CancelInvoke("ReturnToSpawn");
                currentState = EnemyState.CHASE;
            }
		}

        // As long as the player is in the enemy's range, they will continue to chase them
		private void OnTriggerStay(Collider other)
		{
            if(other.CompareTag("Player") && currentState == EnemyState.CHASE)
            {
                enemyAgent.SetDestination(other.transform.position);
            }
		}

        // When the player leaves the enemy's range, it will stop chasing them
		private void OnTriggerExit(Collider other)
		{
            if(other.CompareTag("Player") && currentState == EnemyState.CHASE)
            {
                currentState = EnemyState.WAIT;
                enemyAgent.isStopped = true;
                Invoke("ReturnToSpawn", timeToReset);
            }
		}

        // Resets the enemy's position
        private void ReturnToSpawn()
        {
            currentState = EnemyState.PATROL;
            enemyAgent.isStopped = false;
            enemyAgent.SetDestination(homeSpawn.transform.position);
        }
	}
}

