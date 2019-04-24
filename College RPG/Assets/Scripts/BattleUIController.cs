/*  This controls all of the UI actions that the player can perform
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MattScripts {

    public enum BattleMenuStates {
        INACTIVE,           // The menu is hidden
        MAIN,               // The user is viewing the main attack menu
        ATTACK_TARGET,      // The user is selecting a target to do a normal attack on
        SPECIAL,            // The user is selecting a special attack
        SPECIAL_TARGET,     // The user is selecting a target to do the special attack
        ITEM,               // The user is selecting an item
        ITEM_TARGET,        // The user is selecting a target to use the item on
    }

    [RequireComponent(typeof(BattleController))]
    public class BattleUIController : MonoBehaviour {

        public BattleController battleController;                       // A hard ref to the current battle controller

        [Header("Input Variables")]
        public string selectInput = "Interact";
        public string cancelInput = "Cancel";
        public string scrollInput = "Vertical";
        [Range(0.1f,0.5f)]
        public float scrollDelay = 0.3f;

        [Header("UI Variables")]
        public GameObject battleUI;                                     // Reference to the GameObject that holds all of the UI
        public TMP_ColorGradient selectChoiceHighlight;                 // Reference to the ColorGradiant used to showcase which text the player has selected

        // Private variables
        private BattleMenuStates currentState = BattleMenuStates.INACTIVE;

        private GameObject descriptionBox;                              // The box that is used to show the description of the command
        private GameObject actionBox;                                   // The box that is used to highlight an attack
        private GameObject commandBox;                                  // The box that contains all of the main menu commands
        private GameObject subCommandBox;                               // The box that contains all of the sube commands

        private Transform currentMenuParent = null;                     // The current menu item that contains the list of options

        private Stack<int> prevIndexMenus;                              // Used to save the previous index spaces when navigating menus
        private int currentMenuIndex = 0;
        private bool hasScrolled = false;

        private TextMeshProUGUI descText;                               // Hard refs to store for later usage
        private TextMeshProUGUI actionText;
        private PlayerInventory playerInventory;

        public BattleMenuStates CurrentState {
            get { return currentState; }
            set {
                currentState = value;
            }
        }

        // Activates the UI upon this script activating
		private void Awake()
		{
            battleUI.SetActive(true);
		}

		// We initialize the UI Controller
		private void Start()
		{
            prevIndexMenus = new Stack<int>();

            commandBox = battleUI.transform.GetChild(0).gameObject;
            descriptionBox = battleUI.transform.GetChild(1).gameObject;
            subCommandBox = battleUI.transform.GetChild(2).gameObject;
            actionBox = battleUI.transform.GetChild(3).gameObject;
            descText = descriptionBox.GetComponentInChildren<TextMeshProUGUI>();
            actionText = actionBox.GetComponentInChildren<TextMeshProUGUI>();
            currentMenuParent = commandBox.transform;

            HideMenus();
		}

		// Checks for the player input depending on the specific states
		private void Update()
        {
            // If we are NOT hidden, we enact on the rest of the pause menu logic
            if(currentState != BattleMenuStates.INACTIVE)
            {
                // Handles moving the player input up and down when the player is selecting an option
                if(Input.GetAxis(scrollInput) > 0f && currentMenuIndex > 0 && CheckIfOptionIsValid(currentMenuIndex - 1))
                {
                    // We delay the scroll so that it is easier to navigate the menus
                    if(hasScrolled == false)
                    {
                        ChangeSelectedText(currentMenuIndex, currentMenuIndex - 1);
                        currentMenuIndex--;
                        UpdateMenuContext();
                        hasScrolled = true;
                        Invoke("ResetScroll", scrollDelay);
                    }
                }
                else if(Input.GetAxis(scrollInput) < 0f && currentMenuIndex + 1 < currentMenuParent.childCount && CheckIfOptionIsValid(currentMenuIndex + 1))
                {
                    // We delay the scroll so that it is easier to navigate the menus
                    if(hasScrolled == false)
                    {
                        ChangeSelectedText(currentMenuIndex, currentMenuIndex + 1);
                        currentMenuIndex++;
                        UpdateMenuContext();
                        hasScrolled = true;
                        Invoke("ResetScroll", scrollDelay);
                    }
                }

                // Logic for selecting a field
                if(Input.GetButtonDown(selectInput))
                {
                    Debug.Log("We selected " + currentMenuParent.GetChild(currentMenuIndex).GetComponent<TextMeshProUGUI>().text);

                    bool success = SelectOption();
                    if(success == true)
                    {
                        UpdateMenuContext();
                    }
                }
                // Logic for returning to the last selected item
                else if(Input.GetButtonDown(cancelInput))
                {
                    ReturnToPreviousOption();
                    UpdateMenuContext();
                }
            }
        }

        // Depending on what menu we are in, we update what is currently displayed after we scroll on something
        private void UpdateMenuContext()
        {
            // If we do not have this saved at this point, we will save a reference to these.
            if(playerInventory == null)
            {
                playerInventory = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();
            }

            switch(currentState)
            {
                case BattleMenuStates.MAIN:
                    // Updates the description box based on the highlighted action
                    if(currentMenuIndex == 0)
                    {
                        descText.text = "Perform a standard attack on the selected enemy.";
                    }
                    else if(currentMenuIndex == 1)
                    {
                        descText.text = "Perform a special attack on the selected enemy.";
                    }
                    else
                    {
                        descText.text = "Use a battle item to heal oneself or another party member.";
                    }
                    break;
                case BattleMenuStates.ATTACK_TARGET:
                case BattleMenuStates.SPECIAL_TARGET:
                    descText.text = "Select a target.";
                    break;
                case BattleMenuStates.ITEM_TARGET:
                    descText.text = "Who to use this on?";
                    break;
                case BattleMenuStates.SPECIAL:
                    descText.text = ((CharacterData)battleController.GetCurrentCharacterInTurnOrder().battleData).demonData.attackList[currentMenuIndex].attackDescription;
                    break;
                case BattleMenuStates.ITEM:
                    if(playerInventory.GetItemAtIndex(currentMenuIndex) != null)
                    {
                        descText.text = playerInventory.GetItemAtIndex(currentMenuIndex).SpecifiedItem.itemDescription;
                    }
                    else
                    {
                        descText.text = "No items available...";
                    }
                    break;
            }
        }

        // Depending what state we are in, we handle how to handle the menu logic from here after selecting something
        // Returns true if we were able to get a successful option
        private bool SelectOption()
        {
            // We first check if we have any special cases before an option is selected. If we meet any, we exit out of the method early
            switch(currentState)
            {
                case BattleMenuStates.SPECIAL:
                    if(battleController.GetCurrentCharacterInTurnOrder().CheckIfCanUseAttack(currentMenuIndex) == false)
                    {
                        // If we are selecting a special move, but we don't have enough sp, we prevent the player from using it
                        descText.text = "Can't use that attack! Not enough SP!";
                        return false;
                    }
                    break;
            }

            // Once we reach here, we will presume that the option will work
            // So we set up the next menus for the new context
            ChangeSelectedText(currentMenuIndex, -1);
            DeactivateSubMenuItems();

            switch(currentState)
            {
                case BattleMenuStates.MAIN:
                    // We make the sub menu visible
                    subCommandBox.SetActive(true);
                    currentMenuParent = subCommandBox.transform.GetChild(0).GetChild(0);

                    // We then update the display
                    if(currentMenuIndex == 0)
                    {
                        // We select the attack option
                        FillEnemyTargetMenu();
                        currentState = BattleMenuStates.ATTACK_TARGET;
                    }
                    else if(currentMenuIndex == 1)
                    {
                        // We select the special option
                        FillSubMenu(battleController.GetCurrentCharacterInTurnOrder());
                        currentState = BattleMenuStates.SPECIAL;
                    }
                    else
                    {
                        // We select the item option
                        FillSubMenu();
                        currentState = BattleMenuStates.ITEM;
                    }
                    break;
                case BattleMenuStates.ATTACK_TARGET:
                    // We have confirmed an action to attack
                    HideMenus();
                    StartCoroutine(battleController.PerformAttackAction(prevIndexMenus.Peek(), battleController.GetSpecificEnemy(currentMenuIndex)));
                    currentState = BattleMenuStates.INACTIVE;
                    break;
                case BattleMenuStates.SPECIAL:
                    // We have confirmed what special attack we want to do
                    FillEnemyTargetMenu();
                    currentState = BattleMenuStates.SPECIAL_TARGET; 
                    break;
                case BattleMenuStates.SPECIAL_TARGET:
                    // We have confirmed our target to hit our attack on
                    HideMenus();
                    StartCoroutine(battleController.PerformAttackAction(prevIndexMenus.Peek(), battleController.GetSpecificEnemy(currentMenuIndex)));
                    currentState = BattleMenuStates.INACTIVE;
                    break;
                case BattleMenuStates.ITEM:
                    // We confirmed what item to use
                    FillPartyTargetMenu();
                    currentState = BattleMenuStates.ITEM_TARGET;
                    break;
                case BattleMenuStates.ITEM_TARGET:
                    // We have confirmed who to use the item on
                    HideMenus();
                    StartCoroutine(battleController.PerformItemAction(playerInventory.GetItemAtIndex(prevIndexMenus.Peek()), battleController.GetSpecificPartyMember(currentMenuIndex)));
                    currentState = BattleMenuStates.INACTIVE;
                    break;
            }

            prevIndexMenus.Push(currentMenuIndex);
            currentMenuIndex = 0;
            ChangeSelectedText(currentMenuIndex, 0);
            return true;
        }

        // Depending on where we are, we hop back to the previous option
        private void ReturnToPreviousOption()
        {
            if(prevIndexMenus.Count > 0)
            {
                ChangeSelectedText(currentMenuIndex, -1);
                DeactivateSubMenuItems();

                switch(currentState)
                {
                    case BattleMenuStates.ATTACK_TARGET:
                    case BattleMenuStates.ITEM:
                    case BattleMenuStates.SPECIAL:
                        subCommandBox.gameObject.SetActive(false);
                        currentMenuParent = battleUI.transform.GetChild(0);
                        currentState = BattleMenuStates.MAIN;
                        break;
                    case BattleMenuStates.ITEM_TARGET:
                        FillSubMenu();
                        currentState = BattleMenuStates.ITEM;
                        break;
                    case BattleMenuStates.SPECIAL_TARGET:
                        FillSubMenu(battleController.GetCurrentCharacterInTurnOrder());
                        currentState = BattleMenuStates.SPECIAL;
                        break;
                }

                int newIndex = prevIndexMenus.Pop();
                ChangeSelectedText(currentMenuIndex, newIndex);
                currentMenuIndex = newIndex;
            }
        }

        // We fill in all of the items in the sub menus
        // An overloaded method
        private void FillSubMenu()
        {
            int currIndex = 0;
            while(currIndex < playerInventory.GetItemInventorySize())
            {
                ItemData currItem = playerInventory.GetItemAtIndex(currIndex).SpecifiedItem;
                if(currItem.itemType != ItemType.KEY_ITEM)
                {
                    currentMenuParent.GetChild(currIndex).GetComponent<TextMeshProUGUI>().text = currItem.itemName;
                    currentMenuParent.GetChild(currIndex).gameObject.SetActive(true);
                    ++currIndex;
                }
            }
        }
        private void FillSubMenu(BattleStats currentEntity)
        {
            // We extract the attack data from the demon and fill in the menu
            CharacterData currentCharacter = (CharacterData)currentEntity.battleData;

            for(int currIndex = 0; currIndex < currentCharacter.demonData.attackList.Count; ++currIndex)
            {
                AttackData currAttack = currentCharacter.demonData.attackList[currIndex];
                currentMenuParent.GetChild(currIndex).GetComponent<TextMeshProUGUI>().text = currAttack.attackName;
                currentMenuParent.GetChild(currIndex).gameObject.SetActive(true);
            }
        }

        // Fills in all of the available party members in the group
        private void FillPartyTargetMenu()
        {
            for(int currPartyIndex = 0; currPartyIndex < battleController.GetPartySize(); ++currPartyIndex)
            {
                CharacterData currCharacter = (CharacterData)battleController.GetSpecificPartyMember(currPartyIndex).battleData;
                currentMenuParent.GetChild(currPartyIndex).GetComponent<TextMeshProUGUI>().text = currCharacter.characterName;
                currentMenuParent.GetChild(currPartyIndex).gameObject.SetActive(true);
            }
        }

        // Fills in all of the available enemies
        private void FillEnemyTargetMenu()
        {
            for(int currEnemyIndex = 0; currEnemyIndex < battleController.GetEnemySize(); ++currEnemyIndex)
            {
                if(battleController.GetSpecificEnemy(currEnemyIndex).CurrentHP > 0)
                {
                    EnemyData currCharacter = (EnemyData)battleController.GetSpecificEnemy(currEnemyIndex).battleData;
                    currentMenuParent.GetChild(currEnemyIndex).GetComponent<TextMeshProUGUI>().text = currCharacter.enemyName;
                    currentMenuParent.GetChild(currEnemyIndex).gameObject.SetActive(true);   
                }
            }
        }

        // Helper method that changes the two texts in the currentMenuParent at the specified indexes to change gradiants
        // NOTE: if we specify -1 for the newIndex, we change the old index point only.
        private void ChangeSelectedText(int oldIndex, int newIndex)
        {
            if(oldIndex < currentMenuParent.childCount && newIndex < currentMenuParent.childCount)
            {
                currentMenuParent.GetChild(oldIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = null;

                if(newIndex != -1)
                {
                    currentMenuParent.GetChild(newIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = selectChoiceHighlight;
                }
            }
        }

        // Checks if the new index point is a valid spot to move to
        private bool CheckIfOptionIsValid(int newIndex)
        {
            if(currentMenuParent.GetChild(newIndex).gameObject.activeInHierarchy == true)
            {
                return true;
            }
            return false;
        }

        // Called in an invoke to allow for scrolling
        private void ResetScroll()
        {
            hasScrolled = false;
        }
    
        // We deactivate all of the sub menu items for hiding
        private void DeactivateSubMenuItems()
        {
            for(int currIndex = 0; currIndex < subCommandBox.transform.GetChild(0).GetChild(0).childCount; ++currIndex)
            {
                if(subCommandBox.transform.GetChild(0).GetChild(0).GetChild(currIndex).gameObject.activeInHierarchy == false)
                {
                    break;
                }
                else
                {
                    subCommandBox.transform.GetChild(0).GetChild(0).GetChild(currIndex).GetComponent<TextMeshProUGUI>().text = "";
                    subCommandBox.transform.GetChild(0).GetChild(0).GetChild(currIndex).gameObject.SetActive(false);
                }
            }
        }

        // This hides the main commands, the description box, and the sub box when called
        // This does NOT hide the action box
        public void HideMenus()
        {
            if(currentState != BattleMenuStates.INACTIVE)
            {
                ChangeSelectedText(currentMenuIndex, -1);
                commandBox.SetActive(false);
                descriptionBox.SetActive(false);
                subCommandBox.SetActive(false);

                currentState = BattleMenuStates.INACTIVE;
            }
        }

        // This reenables the Command and Description box, resetting the state to the main user state
        public void ShowMenus()
        {
            if(currentState == BattleMenuStates.INACTIVE)
            {
                prevIndexMenus.Clear();

                commandBox.SetActive(true);
                descriptionBox.SetActive(true);

                currentMenuParent = commandBox.transform;
                ChangeSelectedText(currentMenuIndex, 0);
                currentState = BattleMenuStates.MAIN;

                UpdateMenuContext();
                DeactivateSubMenuItems();
            }
        }
    
        // A Helper method that toggles the text box for actions to show a specific text
        public void ToggleActionBox(bool isShowing, string textToAdd = "")
        {
            actionBox.SetActive(isShowing);
            actionText.text = textToAdd;
        }
    }      
}