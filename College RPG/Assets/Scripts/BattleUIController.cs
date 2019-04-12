/*  This controls all of the UI actions that the player can perform
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MattScripts {

    public enum BattleMenuStates {
        HIDDEN,             // The menu is hidden
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
        private BattleMenuStates currentState = BattleMenuStates.HIDDEN;

        private GameObject descriptionBox;                              // The box that is used to show the description of the command
        private GameObject actionBox;                                   // The box that is used to highlight an attack
        private GameObject commandBox;                                  // The box that contains all of the main menu commands
        private GameObject subCommandBox;                               // The box that contains all of the sude commands

        private Transform currentMenuParent = null;                     // The current menu item that contains the list of options

        private Stack<int> prevIndexMenus;                              // Used to save the previous index spaces when navigating menus
        private int currentMenuIndex = 0;
        private int prevSizeOfStack = 0;                                // Used to back the player out of a sub menu
        private bool hasScrolled = false;

        // We initialize the UI Controller
		private void Start()
		{
            prevIndexMenus = new Stack<int>();

            commandBox = battleUI.transform.GetChild(0).gameObject;
            descriptionBox = battleUI.transform.GetChild(1).gameObject;
            subCommandBox = battleUI.transform.GetChild(2).gameObject;
            actionBox = battleUI.transform.GetChild(3).gameObject;
            currentMenuParent = commandBox.transform;

            currentState = BattleMenuStates.MAIN;
            ChangeSelectedText(currentMenuIndex, 0);
            UpdateMenuContext();
		}

		// Checks for the player input depending on the specific states
		private void Update()
        {
            // If we are NOT hidden, we enact on the rest of the pause menu logic
            if(currentState != BattleMenuStates.HIDDEN)
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
                    SelectOption();
                    UpdateMenuContext();
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
            TextMeshProUGUI descText = descriptionBox.GetComponentInChildren<TextMeshProUGUI>();

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
                    descText.text = "Select a target.";
                    break;
                case BattleMenuStates.SPECIAL:
                    // Shows a description of the target
                    break;
                case BattleMenuStates.ITEM:
                    // Shows a description of the item
                    break;
            }
        }

        // Depending what state we are in, we handle how to handle the menu logic from here after selecting something
        private void SelectOption()
        {
            switch(currentState)
            {
                case BattleMenuStates.MAIN:
                    // We change to the new menu and update the display
                    if(currentMenuIndex == 0)
                    {
                        // We select the attack option
                        currentState = BattleMenuStates.ATTACK_TARGET;
                    }
                    else if(currentMenuIndex == 1)
                    {
                        // We select the special option
                        currentState = BattleMenuStates.SPECIAL;
                    }
                    else
                    {
                        // We select the item option
                        currentState = BattleMenuStates.ITEM;
                    }

                    // We then make the sub menu visible
                    currentMenuParent = battleUI.transform.GetChild(2);
                    currentMenuParent.gameObject.SetActive(true);
                    break;
                case BattleMenuStates.ATTACK_TARGET:
                    // We have confirmed an action to attack
                    break;
                case BattleMenuStates.SPECIAL:
                    // We have confirmed what special attack we want to do 
                    break;
                case BattleMenuStates.SPECIAL_TARGET:
                    // We have confirmed our target to hit our attack on
                    break;
                case BattleMenuStates.ITEM:
                    // We confirmed what item to use
                    break;
                case BattleMenuStates.ITEM_TARGET:
                    // We have confirmed who to use the item on
                    break;
            }

            // All of this handles the logic for returning back to the first option
            // We first save the previous index point and change the color of the text to be unselected
            prevSizeOfStack = prevIndexMenus.Count;
            prevIndexMenus.Push(currentMenuIndex);
            ChangeSelectedText(currentMenuIndex, -1);

            // And we then update the selection to that new menu.
            currentMenuIndex = 0;
            ChangeSelectedText(currentMenuIndex, 0);
        }

        // Depending on where we are, we hop back to the previous option
        private void ReturnToPreviousOption()
        {
            if(prevIndexMenus.Count > 0)
            {
                if(prevIndexMenus.Count - 1 == prevSizeOfStack)
                {
                    currentMenuParent.gameObject.SetActive(false);
                    ChangeSelectedText(currentMenuIndex, -1);

                    switch(currentState)
                    {
                        case BattleMenuStates.ATTACK_TARGET:
                        case BattleMenuStates.ITEM:
                        case BattleMenuStates.SPECIAL:
                            currentMenuParent = battleUI.transform.GetChild(0);
                            currentState = BattleMenuStates.MAIN;
                            break;
                        case BattleMenuStates.ITEM_TARGET:
                            currentState = BattleMenuStates.ITEM;
                            // TODO: Determine how to get back from selecting the target to selecting the item/move
                            break;
                        case BattleMenuStates.SPECIAL_TARGET:
                            currentState = BattleMenuStates.SPECIAL;
                            // TODO: Determine how to get back from selecting the target to selecting the item/move
                            break;
                    }

                    currentMenuParent.gameObject.SetActive(true);
                }

                int newIndex = prevIndexMenus.Pop();
                ChangeSelectedText(currentMenuIndex, newIndex);
                currentMenuIndex = newIndex;
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
            if(currentMenuParent.GetChild(newIndex).GetComponent<TextMeshProUGUI>().text == "")
            {
                return false;
            }
            return true;
        }

        // Called in an invoke to allow for scrolling
        private void ResetScroll()
        {
            hasScrolled = false;
        }
    }      
}