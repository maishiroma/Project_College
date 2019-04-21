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

        public BattleUIController battleUIController;

        [Header("General Variables")]
        public Transform[] partySpawnLocations;
        public Transform[] enemySpawnLocations;

        [Header("Prefabs")]
        public GameObject characterPrefab;

        // Private Variables
        [SerializeField]
        private BattleStates currentState;
        private BattleEvent currentBattleEvent;
        private BattleStats[] listOfAllParty;
        private BattleStats[] listOfAllEnemies;
        private BattleStats[] listOfAllEntitiesInTurnOrder;
        private int currentTurnIndex;

        public BattleStates CurrentState {
            get {return currentState;}
        }

        // Sets the current turn order
        public int SetCurrentTurnOrder {
            set {
                if(value >= listOfAllEntitiesInTurnOrder.Length || value < 0)
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
            listOfAllEntitiesInTurnOrder = DetermineOrderOfAttacks();
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
            listOfAllParty = new BattleStats[playerInventory.GetPartyInvetorySize()];

            int currLocationIndex = 0;
            for(int currIndex = 0; currIndex < playerInventory.GetPartyInvetorySize(); ++currIndex)
            {
                GameObject partyMember = Instantiate(characterPrefab, partySpawnLocations[currLocationIndex].position, Quaternion.identity);
                partyMember.GetComponent<BattleStats>().battleData = playerInventory.GetInventoryCharacterAtIndex(currIndex).SpecifiedCharacter;
                partyMember.GetComponent<BattleStats>().InitializeHPSP(playerInventory.GetInventoryCharacterAtIndex(currIndex).CurrentHealthPoints, 
                                                                       playerInventory.GetInventoryCharacterAtIndex(currIndex).CurrentSkillPoints, 
                                                                       playerInventory.GetInventoryCharacterAtIndex(currIndex).SpecifiedCharacter.maxHealthPoints,
                                                                       playerInventory.GetInventoryCharacterAtIndex(currIndex).SpecifiedCharacter.maxSkillPoints);
                listOfAllParty[currIndex] = partyMember.GetComponent<BattleStats>();
                currLocationIndex++;
            }
        }

        // Spawns in enemies from the event
        private void SpawnEnemies()
        {
            listOfAllEnemies = new BattleStats[currentBattleEvent.listOfEnemiesInFight.Count];

            int currSpawnIndex = 0;
            foreach(EnemyData currentData in currentBattleEvent.listOfEnemiesInFight)
            {
                GameObject newEnemy = Instantiate(characterPrefab, enemySpawnLocations[currSpawnIndex].position, Quaternion.identity);
                newEnemy.GetComponent<BattleStats>().battleData = currentData;
                newEnemy.GetComponent<BattleStats>().InitializeHPSP(currentData.maxHealthPoints, currentData.maxSkillPoints);

                listOfAllEnemies[currSpawnIndex] = newEnemy.GetComponent<BattleStats>();
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
                if(currEntity.IsDead == true && currEntity.battleData is CharacterData)
                {
                    partyDeath++;
                    if(partyDeath == listOfAllParty.Length)
                    {
                        return -1;
                    }
                }
                else if(currEntity.IsDead == true && currEntity.battleData is EnemyData)
                {
                    enemyDeath++;
                    if(enemyDeath == listOfAllEnemies.Length)
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
            if(currentTurnIndex < listOfAllEntitiesInTurnOrder.Length)
            {
                return listOfAllEntitiesInTurnOrder[currentTurnIndex];
            }
            return null;
        }

        // Gets a specific party member
        public BattleStats GetSpecificPartyMember(int index)
        {
            if(index < 0 || index > listOfAllParty.Length)
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
            if(index < 0 || index > listOfAllEnemies.Length)
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
            return listOfAllParty.Length;
        }

        // Returns the size of the entire enemy side in battle
        public int GetEnemySize()
        {
            return listOfAllEnemies.Length;
        }

        // Performs an attack action using the current character on the targeted character
        public IEnumerator PerformAttackAction(int attackIndex, BattleStats targetedCharacter)
        {
            if(GetCurrentCharacterInTurnOrder().battleData is CharacterData)
            {
                // Player atacks enemy
                CharacterData currentCharacter = (CharacterData)GetCurrentCharacterInTurnOrder().battleData;
                DemonData currentDemon = currentCharacter.demonData;
                AttackData currentAttack = currentCharacter.demonData.attackList[attackIndex];

                // Sets the text box to show the attack
                battleUIController.ToggleActionBox(true, currentAttack.attackName);

                // TODO: Associate animation with character
                //GetCurrentCharacterInTurnOrder().gameObject.GetComponent<Animator> currentDemon.attackList[attackIndex].attackAnimation;
                //yield return new WaitForSeconds(5f);

                // Deal damage
                int attackPower = currentCharacter.baseAttack + currentAttack.attackPower;
                int defensePower = 0;
                if(currentAttack.attackType == AttackType.PHYSICAL)
                {
                    attackPower += currentDemon.phyAttackStat;
                    defensePower += ((EnemyData)targetedCharacter.battleData).phyDefenseStat;

                    targetedCharacter.CurrentHP -= attackPower - defensePower;
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    attackPower += currentDemon.spAttackStat;
                    defensePower += ((EnemyData)targetedCharacter.battleData).spDefenseStat;

                    targetedCharacter.CurrentHP -= (attackPower - defensePower);
                    GetCurrentCharacterInTurnOrder().CurrentSP -= currentAttack.attackCost;
                }
                Debug.Log(((EnemyData)targetedCharacter.battleData).enemyName + " took " + (attackPower - defensePower) + " damage!");
                yield return new WaitForSeconds(1);

                // Hide box
                battleUIController.ToggleActionBox(false);
                yield return null;
            }
            else if(GetCurrentCharacterInTurnOrder().battleData is EnemyData)
            {
                // Enemy attacks player
                EnemyData currentEnemy = (EnemyData)GetCurrentCharacterInTurnOrder().battleData;
                DemonData targetedCharacterDemon = ((CharacterData)targetedCharacter.battleData).demonData;
                AttackData currentAttack = currentEnemy.attackList[attackIndex];

                // Sets the text box to show the attack
                battleUIController.ToggleActionBox(true, currentAttack.attackName);

                // TODO: Associate animation with character
                //GetCurrentCharacterInTurnOrder().gameObject.GetComponent<Animator> currentDemon.attackList[attackIndex].attackAnimation;
                //yield return new WaitForSeconds(5f);

                // Deal damage
                int attackPower = currentAttack.attackPower;
                int defensePower = ((CharacterData)targetedCharacter.battleData).baseDefense;
                if(currentAttack.attackType == AttackType.PHYSICAL)
                {
                    attackPower += currentEnemy.phyAttackStat;
                    defensePower += targetedCharacterDemon.phyDefenseStat;

                    targetedCharacter.CurrentHP -= (attackPower - defensePower);
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    attackPower += currentEnemy.spAttackStat;
                    defensePower += targetedCharacterDemon.spDefenseStat;

                    targetedCharacter.CurrentHP -= attackPower - defensePower;
                    GetCurrentCharacterInTurnOrder().CurrentSP -= currentAttack.attackCost;
                }
                Debug.Log(((CharacterData)targetedCharacter.battleData).characterName + " took " + (attackPower - defensePower) + " damage!");
                yield return new WaitForSeconds(1);

                // Hide box
                battleUIController.ToggleActionBox(false);
                yield return null;
            }

            // Check if the target is dead
            if(targetedCharacter.CurrentHP <= 0)
            {
                // One of the player characters is dead, so we deactivate them
                // For now, we hide the enemy sprite
                targetedCharacter.IsDead = true;
                targetedCharacter.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
                yield return new WaitForSeconds(1f);

                // TODO: Do some animation of the character dying
            }

            // We check if a side is wiped, else we continue
            switch(CheckWhoWon())
            {
                case 0:
                    // No one lost yet so We then increment the turn order and change to the next turn
                    SetCurrentTurnOrder = currentTurnIndex + 1;
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
                    break;
                case 1:
                    // The player won, so we change to the player victory
                    battleUIController.ToggleActionBox(true, "You won!");
                    yield return new WaitForSeconds(2f);

                    currentState = BattleStates.PLAYER_WIN;
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
                Debug.Log("Restore " + currentItem.SpecifiedItem.itemAmount + " HP.");
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
                int randPartyMember = Random.Range(0, listOfAllParty.Length);

                while(GetSpecificPartyMember(randPartyMember).IsDead == true)
                {
                    randPartyMember = Random.Range(0, listOfAllParty.Length);
                }

                // We then call PerformAttackAction
                StartCoroutine(PerformAttackAction(randAttackIndex, GetSpecificPartyMember(randPartyMember)));
            }
        }
    }
}