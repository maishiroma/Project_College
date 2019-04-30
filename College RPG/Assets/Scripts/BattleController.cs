/* This script handles all of the global battle logic, from handling the states in the battle, to determining the actions taken at each
 * state.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MattScripts {

    // These are all of the various states a battle can be in.
    public enum BattleStates {
        START,
        PLAYER_TURN,
        ENEMY_TURN,
        PLAYER_WIN,
        ENEMY_WIN
    }

    [RequireComponent(typeof(BattleUIController))]
    public class BattleController : MonoBehaviour {

        [Header("General Variables")]
        public Transform[] partySpawnLocations;
        public Transform[] enemySpawnLocations;
        [Range(1f,5f)]
        public float turnIndicatorYOffset = 1f;

        [Header("Prefabs")]
        public GameObject characterPrefab;

        [Header("External References")]
        public BattleUIController battleUIController;
        public GameObject turnIndicator;

        // Private Variables
        [SerializeField]
        private BattleStates currentState;
        private BattleEvent currentBattleEvent;
        private List<BattleStats> listOfAllParty;
        private List<BattleStats> listOfAllEnemies;
        private List<BattleStats> listOfAllEntitiesInTurnOrder;
        private int currentTurnIndex;

        public BattleStates CurrentState {
            get {return currentState;}
        }

        // Sets the current turn order
        public int SetCurrentTurnOrder {
            set {
                if(value >= listOfAllEntitiesInTurnOrder.Count || value < 0)
                {
                    currentTurnIndex = 0;
                }
                else
                {
                    currentTurnIndex = value;
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
            DetermineOrderOfAttacks();
            currentTurnIndex = 0;

            currentState = BattleStates.START;
            GameManager.Instance.CurrentState = GameStates.BATTLE;
		}

        // On each frame, we run the corresponding logic depending on what state we are in.
        private void Update()
        {
            switch(currentState)
            {
                case BattleStates.START:
                    // We determine who goes first
                    if(GetCurrentCharacterInTurnOrder().battleData is CharacterData)
                    {
                        currentState = BattleStates.PLAYER_TURN;
                        battleUIController.ShowMenus();
                    }
                    else if(GetCurrentCharacterInTurnOrder().battleData is EnemyData)
                    {
                        currentState = BattleStates.ENEMY_TURN;
                        EnemyAI();
                    }

                    // We shift the turn indicator to be on the current character
                    turnIndicator.transform.position = GetCurrentCharacterInTurnOrder().gameObject.transform.position;
                    turnIndicator.transform.position += new Vector3(0,turnIndicatorYOffset,0);
                    break;
            }
        }

        // Determines the order of who goes first
        // First in array = fastest; last = slowest
        // Uses insertion sort
        private void DetermineOrderOfAttacks()
        {
            listOfAllEntitiesInTurnOrder = new List<BattleStats>(FindObjectsOfType<BattleStats>());
            for(int compareIndex = 0; compareIndex < listOfAllEntitiesInTurnOrder.Count; ++compareIndex)
            {
                for(int iteratorIndex = compareIndex + 1; iteratorIndex < listOfAllEntitiesInTurnOrder.Count; ++iteratorIndex)
                {
                    bool checkIfOtherIsParty = (listOfAllEntitiesInTurnOrder[compareIndex].battleData is CharacterData && listOfAllEntitiesInTurnOrder[iteratorIndex].CompareSpeeds((CharacterData)listOfAllEntitiesInTurnOrder[compareIndex].battleData) == true);
                    bool checkIfOtherIsEnemy = (listOfAllEntitiesInTurnOrder[compareIndex].battleData is EnemyData && listOfAllEntitiesInTurnOrder[iteratorIndex].CompareSpeeds((EnemyData)listOfAllEntitiesInTurnOrder[compareIndex].battleData) == true);

                    // We check to see if we the current entity is faster than the current iterator
                    if (checkIfOtherIsParty == true || checkIfOtherIsEnemy == true)
                    {
                        BattleStats temp = listOfAllEntitiesInTurnOrder[iteratorIndex];
                        listOfAllEntitiesInTurnOrder[iteratorIndex] = listOfAllEntitiesInTurnOrder[compareIndex];
                        listOfAllEntitiesInTurnOrder[compareIndex] = temp;
                    }
                }
            }
        }

        // Spawns the party into battle.
        private void SpawnParty()
        {
            PlayerInventory playerInventory = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();
            listOfAllParty = new List<BattleStats>();

            int currLocationIndex = 0;
            for(int currIndex = 0; currIndex < playerInventory.GetPartyInvetorySize(); ++currIndex)
            {
                GameObject partyMember = Instantiate(characterPrefab, partySpawnLocations[currLocationIndex].position, Quaternion.identity);
                partyMember.GetComponent<BattleStats>().battleData = playerInventory.GetInventoryCharacterAtIndex(currIndex).SpecifiedCharacter;
                partyMember.GetComponent<BattleStats>().InitializeHPSP(playerInventory.GetInventoryCharacterAtIndex(currIndex).CurrentHealthPoints, 
                                                                       playerInventory.GetInventoryCharacterAtIndex(currIndex).CurrentSkillPoints, 
                                                                       playerInventory.GetInventoryCharacterAtIndex(currIndex).SpecifiedCharacter.maxHealthPoints,
                                                                       playerInventory.GetInventoryCharacterAtIndex(currIndex).SpecifiedCharacter.maxSkillPoints);
                listOfAllParty.Add(partyMember.GetComponent<BattleStats>());
                currLocationIndex++;
            }
        }

        // Spawns in enemies from the event
        private void SpawnEnemies()
        {
            listOfAllEnemies = new List<BattleStats>();

            int currSpawnIndex = 0;
            foreach(EnemyData currentData in currentBattleEvent.listOfEnemiesInFight)
            {
                GameObject newEnemy = Instantiate(characterPrefab, enemySpawnLocations[currSpawnIndex].position, Quaternion.identity);
                newEnemy.GetComponent<BattleStats>().battleData = currentData;
                newEnemy.GetComponent<BattleStats>().InitializeHPSP(currentData.maxHealthPoints, currentData.maxSkillPoints);

                listOfAllEnemies.Add(newEnemy.GetComponent<BattleStats>());
                currSpawnIndex++;
            }
        }
    
        // Checks if any side lost all of their characters
        // Returns 1 if the player won, -1 if the player lost, or 0 if no one lost
        private int CheckWhoWon()
        {
            int partyDeath = 0;
            int enemyDeath = 0;
            foreach(BattleStats currEntity in listOfAllEntitiesInTurnOrder)
            {
                if(currEntity.CurrentHP <= 0 && currEntity.battleData is CharacterData)
                {
                    partyDeath++;
                    if(partyDeath == listOfAllParty.Count)
                    {
                        return -1;
                    }
                }
                else if(currEntity.CurrentHP <= 0 && currEntity.battleData is EnemyData)
                {
                    enemyDeath++;
                    if(enemyDeath == listOfAllEnemies.Count)
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }

        // Returns the current character that is in the turn order
        public BattleStats GetCurrentCharacterInTurnOrder()
        {
            if(currentTurnIndex < listOfAllEntitiesInTurnOrder.Count)
            {
                return listOfAllEntitiesInTurnOrder[currentTurnIndex];
            }
            return null;
        }

        // Gets a specific party member
        public BattleStats GetSpecificPartyMember(int index)
        {
            if(index < 0 || index > listOfAllParty.Count)
            {
                return null;
            }
            else
            {
                return listOfAllParty[index];
            }
        }

        // Gets a specific enemy
        public BattleStats GetSpecificEnemy(int index)
        {
            if(index < 0 || index > listOfAllEnemies.Count)
            {
                return null;
            }
            else
            {
                return listOfAllEnemies[index];
            }
        }
    
        // Returns the size of the entire party in battle
        public int GetPartySize()
        {
            return listOfAllParty.Count;
        }

        // Returns the size of the entire enemy side in battle
        public int GetEnemySize()
        {
            return listOfAllEnemies.Count;
        }

        // Performs an attack action using the current character on the targeted character
        public IEnumerator PerformAttackAction(int attackIndex, BattleStats targetedCharacter)
        {
            AttackData currentAttack = null;
            string nameOfAttacker = "";
            string nameOfTarget = "";
            int damageDealt = 0;

            if(GetCurrentCharacterInTurnOrder().battleData is CharacterData)
            {
                // Player atacks enemy
                CharacterData currentCharacter = (CharacterData)GetCurrentCharacterInTurnOrder().battleData;
                EnemyData targeted = (EnemyData)targetedCharacter.battleData;

                currentAttack = currentCharacter.demonData.attackList[attackIndex];
                nameOfAttacker = currentCharacter.characterName;
                nameOfTarget = targeted.enemyName;

                // Sets the text box to show the attack
                battleUIController.ToggleActionBox(true, currentAttack.attackName);

                // TODO: Associate animation with character
                //GetCurrentCharacterInTurnOrder().gameObject.GetComponent<Animator> currentDemon.attackList[attackIndex].attackAnimation;
            }
            else if(GetCurrentCharacterInTurnOrder().battleData is EnemyData)
            {
                // Enemy attacks player
                EnemyData currentEnemy = (EnemyData)GetCurrentCharacterInTurnOrder().battleData;
                CharacterData targeted = (CharacterData)targetedCharacter.battleData;

                currentAttack = currentEnemy.attackList[attackIndex];
                nameOfAttacker = currentEnemy.enemyName;
                nameOfTarget = targeted.characterName;

                // Sets the text box to show the attack
                battleUIController.ToggleActionBox(true, currentAttack.attackName);

                // TODO: Associate animation with character
                //GetCurrentCharacterInTurnOrder().gameObject.GetComponent<Animator> currentDemon.attackList[attackIndex].attackAnimation;
            }
            yield return new WaitForSeconds(1f);

            // Deal damage
            damageDealt = targetedCharacter.DealDamage(GetCurrentCharacterInTurnOrder(), currentAttack);
            switch(targetedCharacter.GetAttackEffectiveness(currentAttack))
            {
                case AffinityValues.NORMAL:
                    battleUIController.ToggleActionBox(true, nameOfAttacker + " delt " + damageDealt + " damage to " + nameOfTarget + ".");
                    break;
                case AffinityValues.WEAK:
                    battleUIController.ToggleActionBox(true, nameOfAttacker + " hits " + nameOfTarget + "'s weakspot for " + damageDealt + " damage!");
                    break;
                case AffinityValues.RESISTANT:
                    battleUIController.ToggleActionBox(true, nameOfTarget + " resisted " + nameOfTarget + "'s move...only dealt " + damageDealt + " damage.");
                    break;
                case AffinityValues.NULL:
                    battleUIController.ToggleActionBox(true, nameOfTarget + " is immune to " + nameOfTarget + "'s attack...did " + damageDealt + " damage.");
                    break;
            }
            yield return new WaitForSeconds(1f);

            // Checks if the target is dead
            if(targetedCharacter.CurrentHP <= 0)
            {
                battleUIController.ToggleActionBox(true, nameOfTarget + " is defeated!");

                // We move it to the back of the line in the turn order and deactivate it.
                if(targetedCharacter.battleData is CharacterData)
                {
                    targetedCharacter.gameObject.SetActive(false);
                    listOfAllParty.Remove(targetedCharacter);
                    listOfAllParty.Add(targetedCharacter);
                }
                else if(targetedCharacter.battleData is EnemyData)
                {
                    targetedCharacter.gameObject.SetActive(false);
                    listOfAllEnemies.Remove(targetedCharacter);
                    listOfAllEnemies.Add(targetedCharacter);
                }
                listOfAllEntitiesInTurnOrder.Remove(targetedCharacter);
                listOfAllEntitiesInTurnOrder.Add(targetedCharacter);
            }
            yield return new WaitForSeconds(1f);

            // We check if a side is wiped, else we continue
            switch(CheckWhoWon())
            {
                case 0:
                    // No one lost yet so we then increment the turn order and change to the next turn
                    SetCurrentTurnOrder = currentTurnIndex + 1;
                    while(GetCurrentCharacterInTurnOrder().CurrentHP <= 0)
                    {
                        SetCurrentTurnOrder = currentTurnIndex + 1;
                    }

                    // We shift the turn indicator to be on the current character
                    turnIndicator.transform.position = GetCurrentCharacterInTurnOrder().gameObject.transform.position;
                    turnIndicator.transform.position += new Vector3(0,turnIndicatorYOffset,0);

                    if(GetCurrentCharacterInTurnOrder().battleData is CharacterData)
                    {
                        // Hide action box
                        battleUIController.ToggleActionBox(false);
                        yield return null;

                        battleUIController.ShowMenus();
                        currentState = BattleStates.PLAYER_TURN;
                    }
                    else if(GetCurrentCharacterInTurnOrder().battleData is EnemyData)
                    {
                        battleUIController.CurrentState = BattleMenuStates.INACTIVE;
                        currentState = BattleStates.ENEMY_TURN;
                        EnemyAI();
                    }
                    yield return null;
                    break;
                case 1:
                    // The player won, so we change to the player victory
                    battleUIController.ToggleActionBox(true, "You won!");
                    yield return new WaitForSeconds(2f);

                    currentState = BattleStates.PLAYER_WIN;
                    currentBattleEvent.PostBattleActions(listOfAllParty);
                    currentBattleEvent.EventOutcome();
                    break;
                case -1:
                    // The enemy won, so we change to the enemy victory
                    battleUIController.ToggleActionBox(true, "You lost...");
                    yield return new WaitForSeconds(2f);

                    currentState = BattleStates.ENEMY_WIN;
                    currentBattleEvent.EventOutcome();
                    break;
            }
        }

        // Perform using an item on the selected character
        public IEnumerator PerformItemAction(InventoryItem currentItem, BattleStats targetedCharacter)
        {
            // Display the action box
            battleUIController.ToggleActionBox(true, currentItem.SpecifiedItem.itemName);

            // TODO: Animation for item usage
            yield return new WaitForSeconds(1f);

            // Restore amount
            if(currentItem.SpecifiedItem.itemType == ItemType.HEALTH)
            {
                targetedCharacter.CurrentHP += currentItem.SpecifiedItem.itemAmount;
                Debug.Log("Restore " + currentItem.SpecifiedItem.itemAmount + " to HP.");
            }
            else if(currentItem.SpecifiedItem.itemType == ItemType.SP)
            {
                targetedCharacter.CurrentSP += currentItem.SpecifiedItem.itemAmount;
                Debug.Log("Restore " + currentItem.SpecifiedItem.itemAmount + " SP.");
            }
            GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>().RemoveItemFromInventory(currentItem, 1);
            yield return null;

            battleUIController.ToggleActionBox(false);
            yield return null;

            // We then increment the turn order and change to the next turn
            SetCurrentTurnOrder = currentTurnIndex + 1;

            // We shift the turn indicator to be on the current character
            turnIndicator.transform.position = GetCurrentCharacterInTurnOrder().gameObject.transform.position;
            turnIndicator.transform.position += new Vector3(0,turnIndicatorYOffset,0);

            if(GetCurrentCharacterInTurnOrder().battleData is CharacterData)
            {
                battleUIController.ShowMenus();
                currentState = BattleStates.PLAYER_TURN;
            }
            else if(GetCurrentCharacterInTurnOrder().battleData is EnemyData)
            {
                battleUIController.CurrentState = BattleMenuStates.INACTIVE;
                currentState = BattleStates.ENEMY_TURN;
                EnemyAI();
            }
            yield return null;
        }
    
        // WIP: This method is invoked when it is the enemy's turn.
        // For now, all we do is attack a random character
        public void EnemyAI()
        {
            // We first check to see if we are an enemy
            if(GetCurrentCharacterInTurnOrder().battleData is EnemyData)
            {
                // We then decide on a random attack and a random target
                int randAttackIndex = Random.Range(0, ((EnemyData)GetCurrentCharacterInTurnOrder().battleData).attackList.Count);
                int randPartyMember = Random.Range(0, listOfAllParty.Count);

                while(GetSpecificPartyMember(randPartyMember).CurrentHP <= 0)
                {
                    randPartyMember = Random.Range(0, listOfAllParty.Count);
                }

                // We then call PerformAttackAction
                StartCoroutine(PerformAttackAction(randAttackIndex, GetSpecificPartyMember(randPartyMember)));
            }
        }
    }
}