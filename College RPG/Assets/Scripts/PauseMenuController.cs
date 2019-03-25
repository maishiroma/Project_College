/*  This controls all of the logic that takes place in the PauseMenu.
 *  From navigating the menus, selection options, etc.
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MattScripts {

    // These are all of the states that the menu can potentally be in
    public enum MenuStates {
        HIDDEN, // The menu is closed
        MAIN,
        ITEM,
        PARTY,
        GEAR,
        LINK,
        LOADING // The transition state used to prevent any player input
    }

    public class PauseMenuController : MonoBehaviour {

        [Header("General Variables")]
        public GameObject pauseMenuObject;
        public string scrollInput = "Vertical";
        public string pauseInput = "Pause";
        public string selectInput = "Interact";
        public string cancelInput = "Cancel";
        [Range(0.1f,0.5f)]
        public float scrollDelay = 0.3f;

        [Header("UI Variables")]
        public TMP_ColorGradient selectChoiceHighlight;                 // Reference to the ColorGradiant used to showcase which text the player has selected

        // Private Variables
        private MenuStates currentState = MenuStates.HIDDEN;
        private Stack<int> prevIndexMenus;                              // Used to save the previous index spaces when navigating menus
        private int currentMenuIndex = 0;                               // The current menu index that we are selecting
        private int prevSizeOfStack = 0;                                // Used to back the player out of a sub menu
        private bool hasScrolled = false;

        private Transform currentMenuParent = null;                     // The current menu item that contains the list of options

        private GameObject mainMenuObject;                              // A reference to all of the sub menus
        private GameObject itemMenuObject;
        private GameObject partyMenuObject;
        private GameObject gearMenuObject;
        private GameObject linkMenuObject;

        private PlayerInventory playerInventory;                       // A reference to the player inventory

        // Hides the menu upon startup
		private void Start()
		{
            // Also sets up the private variables
            prevIndexMenus = new Stack<int>();
            mainMenuObject = pauseMenuObject.transform.GetChild(0).gameObject;
            itemMenuObject = pauseMenuObject.transform.GetChild(1).gameObject;
            partyMenuObject = pauseMenuObject.transform.GetChild(2).gameObject;
            gearMenuObject = pauseMenuObject.transform.GetChild(3).gameObject;
            linkMenuObject = pauseMenuObject.transform.GetChild(4).gameObject;

            playerInventory = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();

            pauseMenuObject.transform.localScale = new Vector3(0.1f,0.1f,0.1f);
		}

		// Checks for the player input depending on the specific states
		private void Update()
		{
            if(Input.GetButtonDown(pauseInput))
            {
                if(currentState == MenuStates.HIDDEN && GameManager.Instance.CurrentState == GameStates.NORMAL)
                {
                    // We retrieve the parent that we will traversal on that contains all of the options
                    currentMenuParent = mainMenuObject.transform;
                    ChangeSelectedText(currentMenuIndex, 0);
                    currentMenuIndex = 0;

                    // And then we open the menu, starting at the main menu
                    StartCoroutine(ShowMenu());
                }
                else if(currentState == MenuStates.MAIN)
                {
                    // We hide the menu and resume player control
                    StartCoroutine(HideMenu());
                }
            }

            // If we are NOT hidden, we enact on the rest of the pause menu logic
            if(currentState != MenuStates.HIDDEN && currentState != MenuStates.LOADING)
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
                // Depending on what state we are in, we do various activities
                // TODO: Need to implement the activity withing each field.
                if(Input.GetButtonDown(selectInput))
                {
                    Debug.Log("We selected " + currentMenuParent.GetChild(currentMenuIndex).GetComponent<TextMeshProUGUI>().text);
                    switch(currentState)
                    {
                        case MenuStates.MAIN:
                            MainMenuActions();
                            break;
                        case MenuStates.ITEM:
                        case MenuStates.PARTY:
                        case MenuStates.GEAR:
                        case MenuStates.LINK:
                            break;
                    }
                }
                else if(Input.GetButtonDown(cancelInput))
                {
                    // We return to the last selected itm
                    ReturnToPreviousOption();
                }
            }
		}

        // Depending on what menu we are in, we update what is currently displayed after we scroll on something
        private void UpdateMenuContext()
        {
            switch(currentState)
            {
                case MenuStates.ITEM:
                    // We update the item description
                    if(currentMenuIndex < playerInventory.GetItemInventorySize())
                    {
                        itemMenuObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = playerInventory.GetItemAtIndex(currentMenuIndex).itemDescription;
                    }
                    break;
                case MenuStates.PARTY:
                    // We update the party description
                    if(currentMenuIndex < playerInventory.GetPartyInvetorySize())
                    {
                        partyMenuObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = playerInventory.GetCharacterAtIndex(currentMenuIndex).characterName;
                    }
                    break;
                case MenuStates.GEAR:
                    // We update the gear description
                    if(currentMenuIndex < playerInventory.GetGearInventorySize())
                    {
                        gearMenuObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = playerInventory.GetGearAtIndex(currentMenuIndex).gearDescription;
                    }
                    break;
                case MenuStates.LINK:
                    // We update the link description
                    if(currentMenuIndex < playerInventory.GetLinkInventorySize())
                    {
                        linkMenuObject.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = playerInventory.GetLinkAtIndex(currentMenuIndex).linkDescription;
                    }
                    break;
            }
        }

        // The actions taken if we are in the main menu
        private void MainMenuActions()
        {
            if(currentState == MenuStates.MAIN)
            {
                if(currentMenuIndex == 4)
                {
                    // We exit the menu
                    StartCoroutine(HideMenu());
                }
                else
                {
                    // We disable the main menu, save the previous index, and reset the index to 0
                    mainMenuObject.SetActive(false);
                    prevSizeOfStack = prevIndexMenus.Count;
                    prevIndexMenus.Push(currentMenuIndex);

                    switch(currentMenuIndex)
                    {
                        case 0:
                            // We go to the item menu
                            InitializeItemMenu();
                            break;
                        case 1:
                            // We go to the party menu
                            InitializePartyMenu();
                            break;
                        case 2:
                            // We go to the gear menu
                            InitializeGearMenu();
                            break;
                        case 3:
                            // We go to the link menu
                            InitializeLinkMenu();
                            break;
                    }

                    currentMenuIndex = 0;
                    ChangeSelectedText(currentMenuIndex, 0);
                    UpdateMenuContext();
                }
            }
        }

        // Depending on where we are, we hop back to the previous option
        private void ReturnToPreviousOption()
        {
            if(prevIndexMenus.Count > 0)
            {
                // Right now, we do not have sub menus within menus, so we will be going back to the main menu
                if(prevIndexMenus.Count - 1 == prevSizeOfStack)
                {
                    switch(currentState)
                    {
                        case MenuStates.ITEM:
                            itemMenuObject.SetActive(false);
                            break;
                        case MenuStates.PARTY:
                            partyMenuObject.SetActive(false);
                            break;
                        case MenuStates.GEAR:
                            gearMenuObject.SetActive(false);
                            break;
                        case MenuStates.LINK:
                            linkMenuObject.SetActive(false);
                            break;
                    }

                    ChangeSelectedText(currentMenuIndex, -1);
                    currentMenuParent = pauseMenuObject.transform.GetChild(0);
                    currentMenuParent.gameObject.SetActive(true);
                    currentState = MenuStates.MAIN;
                }

                int newIndex = prevIndexMenus.Pop();
                ChangeSelectedText(currentMenuIndex, newIndex);
                currentMenuIndex = newIndex;
            }
            else
            {
                // We must be at the main menu, so we simply close the menu
                StartCoroutine(HideMenu());
            }
        }

        // We set up all of the variables needed for the item menu
        private void InitializeItemMenu()
        {
            itemMenuObject.SetActive(true);
            currentMenuParent = itemMenuObject.transform.GetChild(0).GetChild(0).GetChild(0);

            // We update all of the items in the menu to reflect the inventory
            int currItemIndex = 0;
            while(currItemIndex < playerInventory.GetItemInventorySize())
            {
                ItemData currItem = playerInventory.GetItemAtIndex(currItemIndex);
                currentMenuParent.GetChild(currItemIndex).GetComponent<TextMeshProUGUI>().text = currItem.itemName;
                ++currItemIndex;
            }
            // For the rest of the items in the list, we clear them from the screen
            while(currItemIndex < currentMenuParent.childCount)
            {
                currentMenuParent.GetChild(currItemIndex).GetComponent<TextMeshProUGUI>().text = "";
                ++currItemIndex;
            }

            currentState = MenuStates.ITEM;
        }

        // We set up all of the variables needed for the party menu
        private void InitializePartyMenu()
        {
            partyMenuObject.SetActive(true);
            currentMenuParent = partyMenuObject.transform.GetChild(0).GetChild(0).GetChild(0);

            int currCharacterIndex = 0;
            while(currCharacterIndex < playerInventory.GetPartyInvetorySize())
            {
                CharacterData currCharacter = playerInventory.GetCharacterAtIndex(currCharacterIndex);
                currentMenuParent.GetChild(currCharacterIndex).GetComponent<TextMeshProUGUI>().text = currCharacter.characterName;
                ++currCharacterIndex;
            }
            // For the rest of the items in the list, we clear them from the screen
            while(currCharacterIndex < currentMenuParent.childCount)
            {
                currentMenuParent.GetChild(currCharacterIndex).GetComponent<TextMeshProUGUI>().text = "";
                ++currCharacterIndex;
            }

            currentState = MenuStates.PARTY;
        }

        // We set up all of the variables needed for the gear menu
        private void InitializeGearMenu()
        {
            gearMenuObject.SetActive(true);
            currentMenuParent = gearMenuObject.transform.GetChild(0).GetChild(0).GetChild(0);

            int currGearIndex = 0;
            while(currGearIndex < playerInventory.GetGearInventorySize())
            {
                GearData currGear = playerInventory.GetGearAtIndex(currGearIndex);
                currentMenuParent.GetChild(currGearIndex).GetComponent<TextMeshProUGUI>().text = currGear.gearName;
                ++currGearIndex;
            }
            // For the rest of the items in the list, we clear them from the screen
            while(currGearIndex < currentMenuParent.childCount)
            {
                currentMenuParent.GetChild(currGearIndex).GetComponent<TextMeshProUGUI>().text = "";
                ++currGearIndex;
            }

            currentState = MenuStates.GEAR;
        }

        // We set up all of the variables needed for the link menu
        private void InitializeLinkMenu()
        {
            linkMenuObject.SetActive(true);
            currentMenuParent = linkMenuObject.transform.GetChild(0).GetChild(0).GetChild(0);

            int currLinkIndex = 0;
            while(currLinkIndex < playerInventory.GetLinkInventorySize())
            {
                LinkData currCharacter = playerInventory.GetLinkAtIndex(currLinkIndex);
                currentMenuParent.GetChild(currLinkIndex).GetComponent<TextMeshProUGUI>().text = currCharacter.linkName;
                ++currLinkIndex;
            }
            // For the rest of the items in the list, we clear them from the screen
            while(currLinkIndex < currentMenuParent.childCount)
            {
                currentMenuParent.GetChild(currLinkIndex).GetComponent<TextMeshProUGUI>().text = "";
                ++currLinkIndex;
            }

            currentState = MenuStates.LINK;
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

        // Opens up the menu to the player
        private IEnumerator ShowMenu()
        {
            // Right now, we just increase/decrease the size
            GameManager.Instance.CurrentState = GameStates.MENU;
            currentState = MenuStates.LOADING;
            pauseMenuObject.SetActive(true);

            while(pauseMenuObject.transform.localScale.x < 1f)
            {
                pauseMenuObject.transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
                yield return null;
            }

            hasScrolled = false;
            currentState = MenuStates.MAIN;
            yield return null;
        }

        // Hides the menu from the player
        private IEnumerator HideMenu()
        {
            // Right now, we just increase/decrease the size
            currentState = MenuStates.LOADING;

            while(pauseMenuObject.transform.localScale.x > 0.1f)
            {
                pauseMenuObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                yield return null;
            }

            // And then we stop showing the menu entirely
            hasScrolled = false;
            pauseMenuObject.SetActive(false);
            currentState = MenuStates.HIDDEN;
            GameManager.Instance.CurrentState = GameStates.NORMAL;
            yield return null;
        }
	}
}