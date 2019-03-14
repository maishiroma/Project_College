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
                    //TODO: Write logic for interacting
                    Debug.Log("We selected " + currentMenuParent.GetChild(currentMenuIndex));
                }
                else if(Input.GetButtonDown(cancelInput))
                {
                    // TODO: Write logic for canceling
                    Debug.Log("We canceled from " + currentMenuParent.GetChild(currentMenuIndex));
                }
            }
		}

        // Helper method that changes the two texts in the currentMenuParent at the specified indexes to change gradiants
        private void ChangeSelectedText(int oldIndex, int newIndex)
        {
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

