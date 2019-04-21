/*  This script handles the game over actions
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MattScripts {

    public class GameOverController : MonoBehaviour {

        [Header("UI Variables")]
        public GameObject gameOverUI;
        public TMP_ColorGradient selectChoiceHighlight;                 // Reference to the ColorGradiant used to showcase which text the player has selected

        [Header("Input Variables")]
        public string selectInput = "Interact";
        public string cancelInput = "Cancel";
        public string scrollInput = "Vertical";
        [Range(0.1f,0.5f)]
        public float scrollDelay = 0.3f;

        // Private Variables
        private TransitionArea transition;
        private Transform currentMenuParent = null;                     // The current menu item that contains the list of options
        private int currentMenuIndex = 0;
        private bool hasScrolled = false;

        // We initialize the UI Controller
        private void Start()
        {
            gameOverUI.SetActive(true);
            currentMenuParent = gameOverUI.transform.GetChild(2);
            transition = gameObject.GetComponent<TransitionArea>();

            ChangeSelectedText(currentMenuIndex,currentMenuIndex);
        }

        // Checks for the player input depending on the specific states
        private void Update()
        {
            // Handles moving the player input up and down when the player is selecting an option
            if(Input.GetAxis(scrollInput) > 0f && currentMenuIndex > 0 && CheckIfOptionIsValid(currentMenuIndex - 1))
            {
                // We delay the scroll so that it is easier to navigate the menus
                if(hasScrolled == false)
                {
                    ChangeSelectedText(currentMenuIndex, currentMenuIndex - 1);
                    currentMenuIndex--;
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
                    hasScrolled = true;
                    Invoke("ResetScroll", scrollDelay);
                }
            }

            // Logic for selecting a field
            if(Input.GetButtonDown(selectInput))
            {
                Debug.Log("We selected " + currentMenuParent.GetChild(currentMenuIndex).GetComponent<TextMeshProUGUI>().text);
                SelectOption();
            }
        }

        // Depending what state we are in, we handle how to handle the menu logic from here after selecting something
        private void SelectOption()
        {
            if(currentMenuIndex == 0)
            {
                // We return to the title screen
                StartCoroutine(transition.GoToSpecificScene(0));
            }
            else if(currentMenuIndex == 1)
            {
                // We quit the game
                StartCoroutine(transition.QuitGame());
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
	}
}
