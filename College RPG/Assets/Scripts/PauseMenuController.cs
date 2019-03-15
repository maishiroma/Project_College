using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MattScripts {

    public enum MenuStates {
        HIDDEN,
        MAIN,
        ITEM,
        PARTY,
        GEAR,
        STATUS,
        LOADING
    }

    public class PauseMenuController : MonoBehaviour {

        [Header("General Variables")]
        public GameObject pauseMenuObject;
        public string pauseInput = "Pause";
        public string selectInput = "Interact";
        public string cancelInput = "Cancel";

        [Header("UI Variables")]
        public TMP_ColorGradient selectChoiceHighlight;                 // Reference to the ColorGradiant used to showcase which text the player has selected

        // Private Variables
        [SerializeField]
        private MenuStates currentState = MenuStates.HIDDEN;
        private Stack<int> prevIndexMenus;                              // Used to save the previous index spaces when navigating menus
        private int currentMenuIndex = 0;                               // The current menu index that we are selecting
        private Transform currentMenuParent = null;                     // The current menu item that contains the list of options

        // Hides the menu upon startup
		private void Start()
		{
            StartCoroutine(HideMenu());
            prevIndexMenus = new Stack<int>();
		}

		// Checks for the player input depending on the specific states
		private void Update()
		{
            if(Input.GetButtonDown(pauseInput))
            {
                if(currentState == MenuStates.HIDDEN && GameManager.Instance.CurrentState == GameStates.NORMAL)
                {
                    // We retrieve the parent that we will traversal on that contains all of the options
                    currentMenuParent = pauseMenuObject.transform.GetChild(0);
                    ChangeSelectedText(currentMenuIndex, 0);
                    currentMenuIndex = 0;

                    // And then we open the menu, starting at the main menu
                    StartCoroutine(ShowMenu());
                }
                else if(currentState == MenuStates.MAIN)
                {
                    // We hide the menu and resume player control
                    StartCoroutine(HideMenu());

                    // TODO: Reset any necessary fields when leaving the pause menu
                }
            }

            // If we are NOT hidden, we enact on the rest of the pause menu logic
            if(currentState != MenuStates.HIDDEN)
            {
                // Handles moving the player input up and down when the player is selecting an option
                if(Input.GetKeyDown(KeyCode.W) && currentMenuIndex > 0)
                {
                    ChangeSelectedText(currentMenuIndex, currentMenuIndex - 1);
                    currentMenuIndex--;
                }
                else if(Input.GetKeyDown(KeyCode.S) && currentMenuIndex + 1 < currentMenuParent.childCount)
                {
                    ChangeSelectedText(currentMenuIndex, currentMenuIndex + 1);
                    currentMenuIndex++;
                }

                // Logic for selecting a field
                if(Input.GetButtonDown(selectInput))
                {
                    switch(currentState)
                    {
                        case MenuStates.MAIN:
                            MainMenuActions();
                            break;
                        case MenuStates.ITEM:
                        case MenuStates.PARTY:
                        case MenuStates.GEAR:
                        case MenuStates.STATUS:
                            break;
                    }
                }
                else if(Input.GetButtonDown(cancelInput))
                {
                    //ReturnToPreviousOption();
                }
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
                    pauseMenuObject.transform.GetChild(0).gameObject.SetActive(false);
                    prevIndexMenus.Push(currentMenuIndex);
                    currentMenuIndex = 0;

                    switch(currentMenuIndex)
                    {
                        case 0:
                            // We go to the item menu
                            pauseMenuObject.transform.GetChild(1).gameObject.SetActive(true);
                            currentMenuParent = pauseMenuObject.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0);
                            ChangeSelectedText(currentMenuIndex, 0);
                            currentState = MenuStates.ITEM;
                            break;
                        case 1:
                            // We go to the party menu
                            pauseMenuObject.transform.GetChild(2).gameObject.SetActive(true);
                            currentMenuParent = pauseMenuObject.transform.GetChild(2);
                            ChangeSelectedText(currentMenuIndex, 0);
                            currentState = MenuStates.PARTY;
                            break;
                        case 2:
                            // We go to the gear menu
                            pauseMenuObject.transform.GetChild(3).gameObject.SetActive(true);
                            currentMenuParent = pauseMenuObject.transform.GetChild(3);
                            ChangeSelectedText(currentMenuIndex, 0);
                            currentState = MenuStates.GEAR;
                            break;
                        case 3:
                            // We go to the status menu
                            pauseMenuObject.transform.GetChild(4).gameObject.SetActive(true);
                            currentMenuParent = pauseMenuObject.transform.GetChild(4);
                            ChangeSelectedText(currentMenuIndex, 0);
                            currentState = MenuStates.STATUS;
                            break;
                    }
                }
            }
        }

        // Depending on where we are, we hop back to the previous option
        private void ReturnToPreviousOption()
        {
            // TODO: make a specific data type that not only keeps track of index pos, but also where that menu item was?
            int newIndex = prevIndexMenus.Pop();
            ChangeSelectedText(currentMenuIndex, newIndex);
            currentMenuIndex = newIndex;
        }

        // Helper method that changes the two texts in the currentMenuParent at the specified indexes to change gradiants
        private void ChangeSelectedText(int oldIndex, int newIndex)
        {
            print(currentMenuParent.GetChild(oldIndex).name);
            currentMenuParent.GetChild(oldIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = null;
            currentMenuParent.GetChild(newIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = selectChoiceHighlight;
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
                yield return new WaitForEndOfFrame();
            }

            currentState = MenuStates.MAIN;
            yield return null;
        }

        // Hides the menu from the player
        private IEnumerator HideMenu()
        {
            // Right now, we just increase/decrease the size
            currentState = MenuStates.LOADING;

            while(pauseMenuObject.transform.localScale.x > 0f)
            {
                pauseMenuObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                yield return new WaitForEndOfFrame();
            }

            pauseMenuObject.SetActive(false);
            currentState = MenuStates.HIDDEN;
            GameManager.Instance.CurrentState = GameStates.NORMAL;
            yield return null;
        }
	}
}

