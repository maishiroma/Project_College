/* This script handles all of the global battle logic, from handling the states in the battle, to determining the actions taken at each
 * state.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    // These are all of the various states a battle can be in.
    public enum BattleStates {
        START,
        PLAYER_TURN,
        ENEMY_TURN,
        PLAYER_WIN,
        ENEMY_WIN
    }

    public class BattleController : MonoBehaviour {

        // Private Variables
        [SerializeField]
        private BattleStates currentState;
        private GameObject[] orderOfAttacks;

        // When the player goes into this scene, the game is set to start the battle
		private void Start()
		{
            currentState = BattleStates.START;
            GameManager.Instance.CurrentState = GameStates.BATTLE;

            // TODO: Spawn in the player party and enemies and fill in their corresponding lists
            // TODO: Find BattleEvent, and fill in the enemy details from there into here
		}

        // On each frame, we run the corresponding logic depending on what state we are in.
		private void Update()
		{
            switch(currentState)
            {
                case BattleStates.START:
                    break;
                case BattleStates.PLAYER_TURN:
                    break;
                case BattleStates.ENEMY_TURN:
                    break;
                case BattleStates.PLAYER_WIN:
                    break;
                case BattleStates.ENEMY_WIN:
                    break;
            }
		}

        // Determines the order of who goes first
        private void DetermineOrderOfAttacks()
        {
            // TODO: Compare the speed stats of the player's party with the enemy, ordering them in the list based on each other's speed
        }
	}
}