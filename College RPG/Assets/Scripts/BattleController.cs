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

        [Header("General Variables")]
        public Transform[] partySpawnLocations;
        public Transform[] enemySpawnLocations;

        [Header("Prefabs")]
        public GameObject characterPrefab;

        // Private Variables
        [SerializeField]
        private BattleStates currentState;
        private BattleEvent currentBattleEvent;
        private BattleStats[] listOfAllCharacters;
        private int currentTurnOrder;

        // Sets the current turn order
        public int SetCurrentTurnOrder {
            set {
                if(value > listOfAllCharacters.Length)
                {
                    currentTurnOrder = 0;
                }
                else if(value < 0)
                {
                    currentTurnOrder = 0;
                }
                else
                {
                    currentTurnOrder = value;
                }
            }
        }

        // When the player goes into this scene, the game is set to start the battle
		private void Start()
		{
            if(FindObjectOfType<BattleEvent>() != null)
            {
                currentBattleEvent = FindObjectOfType<BattleEvent>();
            }

            SpawnParty();
            SpawnEnemies();
            listOfAllCharacters = DetermineOrderOfAttacks();
            currentTurnOrder = 0;

            currentState = BattleStates.START;
            GameManager.Instance.CurrentState = GameStates.BATTLE;
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
        // First in array = fastest; last = slowest
        // Uses insertion sort
        private BattleStats[] DetermineOrderOfAttacks()
        {
            BattleStats[] newOrder = FindObjectsOfType<BattleStats>();
            for(int compareIndex = 0; compareIndex < newOrder.Length; ++compareIndex)
            {
                for(int iteratorIndex = compareIndex + 1; iteratorIndex < newOrder.Length; ++iteratorIndex)
                {
                    if(newOrder[iteratorIndex].CompareSpeeds(newOrder[compareIndex]) == true)
                    {
                        BattleStats temp = newOrder[iteratorIndex];
                        newOrder[iteratorIndex] = newOrder[compareIndex];
                        newOrder[compareIndex] = temp;
                    }
                }
            }
            return newOrder;
        }

        // Spawns the party into battle.
        private void SpawnParty()
        {
            PlayerInventory playerInventory = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();

            int currLocationIndex = 0;
            for(int currIndex = 0; currIndex < playerInventory.GetPartyInvetorySize(); ++currIndex)
            {
                GameObject partyMember = Instantiate(characterPrefab, partySpawnLocations[currLocationIndex].position, Quaternion.identity);
                partyMember.GetComponent<BattleStats>().battleData = playerInventory.GetInventoryCharacterAtIndex(currIndex).SpecifiedCharacter;
                partyMember.GetComponent<BattleStats>().InitializeHPSP(playerInventory.GetInventoryCharacterAtIndex(currIndex).CurrentHealthPoints, playerInventory.GetInventoryCharacterAtIndex(currIndex).CurrentSkillPoints);
                currLocationIndex++;
            }
        }

        // Spawns in enemies from the event
        private void SpawnEnemies()
        {
            int currSpawnIndex = 0;
            foreach(EnemyData currentData in currentBattleEvent.listOfEnemiesInFight)
            {
                GameObject newEnemy = Instantiate(characterPrefab, enemySpawnLocations[currSpawnIndex].position, Quaternion.identity);
                newEnemy.GetComponent<BattleStats>().battleData = currentData;
                newEnemy.GetComponent<BattleStats>().InitializeHPSP(currentData.maxHealthPoints, currentData.maxSkillPoints);
                currSpawnIndex++;
            }
        }
    
        // Returns the current character that is in the turn order
        public BattleStats GetCurrentCharacterInTurnOrder()
        {
            if(currentTurnOrder < listOfAllCharacters.Length)
            {
                return listOfAllCharacters[currentTurnOrder];
            }
            return null;
        }
    }
}