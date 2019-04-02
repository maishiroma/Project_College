/*  This script handles taking the player from the main gameplay to the battle scene
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MattScripts {

    [RequireComponent(typeof(TransitionArea))]
    public class BattleEvent : BaseEvent
    {
        // TODO: Determine how EXP and gold are rewarded after fight
        [Header("Sub Variables")]
        public List<EnemyData> listOfEnemiesInFight;

        // Private Variables
        private Vector3 origPlayerLocation;
        private int origSceneIndex;

        // Upon activating, we start up the battle.
		private void OnEnable()
		{
            if(hasActivated == false)
            {
                EventSetup();
                hasActivated = true;
            }
		}

        // We transition to the battle scene using the required component. 
		public override void EventSetup()
		{
            // We save the original location (There's an issue with this, since the player warps using the GameManager. Need to reword?)
            origPlayerLocation = GameManager.Instance.PlayerReference.transform.position;
            origSceneIndex = SceneManager.GetActiveScene().buildIndex;

            DontDestroyOnLoad(gameObject);
		}

        // We conclude the event and take the player back to the original scene
        public override void EventOutcome()
        {
            // TODO: We first reward the player with their rewards 

            // TODO: And then warp them back using the saved coords and deactivate this event.
            // We acheive this by moving this GameObject onto the player and enabling the box collider.

            // And then we destroy this GameObject

            throw new System.NotImplementedException();
        }
    }
}