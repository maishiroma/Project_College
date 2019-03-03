﻿/*  This derives from BaseEvent, which will open up a dialogue box.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MattScripts {

    public class DialogueEvent : BaseEvent
    {
        [Header("Sub Variables")]
        [Range(0.01f,0.2f)]
        public float typeWriteDelay;                    // Amount of time it takes to display text to the player
        public DialogueNode[] listOfDialogue;           // Array of all of the DialogueNodes that correspond to this event
        public BaseEvent endDialogueEvent;              // Does this Dialogue have an end event?

        [Header("Sub UI Variables")]
        public GameObject playerChoiceUI;               // Reference to the Player Choice UI that contains the UI for player decisions
        public TMP_ColorGradient selectChoiceHighlight; // Reference to the ColorGradiant used to showcase which text the player has selected

        // Private Variables
        private Transform choiceBoxUI;                  // Reference to the UI element that contains all of the choices

        private Animator dialogueWindowAnimator;              // Hard references to all of the UI elements stored for dialouges
        private Animator choiceWindowAnimator;
        private TextMeshProUGUI dialogueUI;
        private TextMeshProUGUI nameUI;
        private Image portaitUI;
        private Image proceedIconUI;

        private int currentDialogueId;                  // The ID of the current dialogue that is being used.
        private int currCharIndex;                      // The character index that AnimateText is currently on when displaying the text             
        private int currChoiceIndex;                    // The index number that is used to identify what choice the player selected
        private int numbOfAvailableChoices;             // Keeps track of the number of choices the player can select from

        private bool hasFinishedPlaying;                // Has this dialogue finished showing all of its text and events?
        private bool hasShownChoices;                   // Has this dialogue shown all of its choices, if necessary

        // Caches the dialouge and image components for future use
		private void Start()
		{
            // The childs in here may change depending on the ordering of the GameObjects in the UI scene
            portaitUI = objectToInteract.transform.GetChild(0).GetComponent<Image>();
            dialogueUI = objectToInteract.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            proceedIconUI = objectToInteract.transform.GetChild(3).GetComponent<Image>();
            nameUI = objectToInteract.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
            dialogueWindowAnimator = objectToInteract.GetComponent<Animator>();

            choiceBoxUI = playerChoiceUI.transform.GetChild(1);
            choiceWindowAnimator = playerChoiceUI.GetComponent<Animator>();
		}

		// Displays the given text to the UI, if the event is activated
		private void Update()
		{
            if(HasActivated)
            {
                if(hasShownChoices == true)
                {
                    // Handles moving the player input up and down when the player is selecting an option
                    if(Input.GetKeyDown(KeyCode.W) && currChoiceIndex > 0)
                    {
                        ChangeSelectedText(currChoiceIndex, currChoiceIndex - 1);
                        currChoiceIndex--;
                    }
                    else if(Input.GetKeyDown(KeyCode.S) && currChoiceIndex + 1 < numbOfAvailableChoices)
                    {
                        ChangeSelectedText(currChoiceIndex, currChoiceIndex + 1);
                        currChoiceIndex++;
                    }
                }

                if(Input.GetKeyDown(interactKey))
                {
                    if(hasShownChoices == true)
                    {
                        // We selected a choice and we are running it
                        StartCoroutine(SelectedChoice());
                    }
                    else
                    {
                        if(hasFinishedPlaying == false)
                        {
                            // We skip the animation and show the entire text
                            currCharIndex = listOfDialogue[currentDialogueId].dialogueText.Length;
                        }
                        else if(listOfDialogue[currentDialogueId].CheckIfChildrenExist() == true)
                        {
                            // We proceed to the next dialogue point
                            currentDialogueId = listOfDialogue[currentDialogueId].GetFirstChildId();
                            StartCoroutine(AnimateText());
                            StartCoroutine(StartDialogueNodeEvent());
                        }
                        else if(endDialogueEvent != null)
                        {
                            // If there is an ending event for this dialogue, we activate it here
                            StartCoroutine(StartDialogueEventEnd());
                        }
                        else
                        {
                            // The dialogue has completed so we stop it
                            EventOutcome();
                        }
                    }
                }
            }
		}

        // Sets up important values that will be needed to be set before the event starts
		public override void EventSetup()
		{
            GameManager.Instance.CurrentState = GameStates.EVENT;
            interactIconUI.SetActive(false);

            dialogueUI.text = "";
            nameUI.text = "";
            portaitUI.sprite = null;
            currentDialogueId = 0;

            dialogueWindowAnimator.SetBool("isOpen", true);
            StartCoroutine(AnimateText());
            StartCoroutine(StartDialogueNodeEvent());
		}

        // When the dialogue is finished, we reset it
		public override void EventOutcome()
        {
            base.EventOutcome();
            GameManager.Instance.CurrentState = GameStates.NORMAL;
            dialogueWindowAnimator.SetBool("isOpen", false);
            Invoke("ResetEvent", 0.5f);
        }

		// When called, will hide the dialogue box without ending the conversation
		public void HideDialogueBox()
        {
            if(dialogueWindowAnimator.GetBool("isOpen") == true)
            {
                dialogueWindowAnimator.SetBool("isOpen", false);
            }
            if(choiceWindowAnimator.GetBool("isOpen") == true)
            {
                choiceWindowAnimator.SetBool("isOpen", false);
            }
        }

        // When called, will show the dialogue box without restarting the conversation
        public void ShowDialogueBox()
        {
            if(dialogueWindowAnimator.GetBool("isOpen") == false)
            {
                dialogueWindowAnimator.SetBool("isOpen", true);
            }
            if(choiceWindowAnimator.GetBool("isOpen") == false)
            {
                choiceWindowAnimator.SetBool("isOpen", true);
            }
        }

        // If we have choices, we fill them out with whatever dialogue node we are on.
        private void FillOptions()
        {
            if(playerChoiceUI != null && listOfDialogue[currentDialogueId].CheckIfChildrenExist())
            {
                // We reset these two vars to 0, since we are filling in new options
                numbOfAvailableChoices = 0;
                currChoiceIndex = 0;

                foreach(int currChildId in listOfDialogue[currentDialogueId].childrenNodeIds)
                {
                    // This check verifies if the current child ID is valid in the current dialogue
                    if(currChildId != -1 && currChildId > 0 && currChildId < listOfDialogue.Length)
                    {
                        // If so, we fill in the UI with the choice that corresponds to that childID
                        choiceBoxUI.GetChild(numbOfAvailableChoices).GetComponent<TextMeshProUGUI>().text = listOfDialogue[currChildId].choiceText;
                        choiceBoxUI.GetChild(numbOfAvailableChoices).gameObject.SetActive(true);
                        numbOfAvailableChoices++;
                    }
                }
                // We then set the first option as the selected choice and activate the window
                choiceBoxUI.GetChild(currChoiceIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = selectChoiceHighlight;
                choiceWindowAnimator.SetBool("isOpen", true);
                hasShownChoices = true;
            }
        }

        // Clears all of the text from the choices
        private void ClearOptions()
        {
            for(int currentIndex = 0; currentIndex < 4; ++currentIndex)
            {
                choiceBoxUI.GetChild(currentIndex).GetComponent<TextMeshProUGUI>().text = "";
                choiceBoxUI.GetChild(currentIndex).gameObject.SetActive(false);
            }
            choiceBoxUI.GetChild(currChoiceIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = null;
            choiceWindowAnimator.SetBool("isOpen", false);
            hasShownChoices = false;
        }

        // Helper method that changes the two texts in the Choice Box at the specified indexes to change gradiants
        private void ChangeSelectedText(int oldIndex, int newIndex)
        {
            choiceBoxUI.GetChild(oldIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = null;
            choiceBoxUI.GetChild(newIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = selectChoiceHighlight;
        }

        // Animates the text to appear in a typewriter fashion!
        private IEnumerator AnimateText()
        {
            // We initialize the animation to start and fill in the name and portraits
            hasFinishedPlaying = false;
            portaitUI.sprite = listOfDialogue[currentDialogueId].portrait;
            nameUI.text = listOfDialogue[currentDialogueId].nameText;
            proceedIconUI.enabled = false;
            yield return new WaitForEndOfFrame();

            // The actual place where the "animation" occues
            for(currCharIndex = 0; currCharIndex < listOfDialogue[currentDialogueId].dialogueText.Length; ++currCharIndex)
            {
                dialogueUI.text = listOfDialogue[currentDialogueId].dialogueText.Substring(0,currCharIndex);
                yield return new WaitForSeconds(typeWriteDelay);
            }

            // We then fill in the rest of the dialogue and enable the rest of the window to appear
            dialogueUI.text = listOfDialogue[currentDialogueId].dialogueText;
            proceedIconUI.enabled = true;
            if(listOfDialogue[currentDialogueId].CheckIfEventExists() == false)
            {
                // If this dialogue has no event, we tell the script that it finished playing out.
                hasFinishedPlaying = true;
            }
            yield return null;

            // If we have choices present, we activate them here
            if(listOfDialogue[currentDialogueId].CheckIfMultipleChilrenExist())
            {
                ClearOptions();
                FillOptions();
                yield return new WaitForEndOfFrame();
            }
        }
    
        // Handles the logic animation for selecting a choice
        private IEnumerator SelectedChoice()
        {
            // All of the choices are listed in the order that they are put in, meaning each choice slot directly corresponds to the 
            // Id slot in the children id array.
            // As such, we can use the currentChoiceIndex to get the approperiate child id to use to advance the dialogue
            currentDialogueId = listOfDialogue[currentDialogueId].childrenNodeIds[currChoiceIndex];
            ClearOptions();
            yield return new WaitForEndOfFrame();

            // We then start the next dialogue using the newly updated dialogueID
            StartCoroutine(AnimateText());
            StartCoroutine(StartDialogueNodeEvent());
            hasShownChoices = false;
            yield return new WaitForEndOfFrame();
        }
    
        // Plays out an event if the specific dialogue node has one
        private IEnumerator StartDialogueNodeEvent()
        {
            if(listOfDialogue[currentDialogueId].CheckIfEventExists() == true)
            {
                BaseEvent currEvent = listOfDialogue[currentDialogueId].endEvent.GetComponent<BaseEvent>();
                if(currEvent.HasActivated == false)
                {
                    listOfDialogue[currentDialogueId].ActivateEvent();
                    yield return new WaitUntil(() => currEvent.HasActivated == false);
                }

                // Once the dialogue has finished playing out, we tell the Script that it can continue
                hasFinishedPlaying = true;
            }
            yield return null;
        }
    
        // Plays out the final event that the dialogue event has one
        private IEnumerator StartDialogueEventEnd()
        {
            endDialogueEvent.gameObject.SetActive(true);
            yield return new WaitUntil(() => endDialogueEvent.HasActivated == false);
            EventOutcome();
        }
    }
}
