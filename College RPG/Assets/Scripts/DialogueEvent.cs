/*  This derives from BaseEvent, which will open up a dialogue box. The player can then read the text that is displayed on the screen.
 *  Can also have options for the player to pick, allowing for branching paths.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MattScripts {

    public class DialogueEvent : BaseEvent
    {
        [Header("Sub Variables")]
        [Range(0.01f,0.2f)]
        public float typeWriteDelay;                    // Amount of time it takes to display text to the player
        public BaseEvent endDialogueEvent;              // Does this Dialogue have an end event?
        public CutSceneEvent associatedCutscene;        // Reference to the cutscene it is associated with one
        public DialogueNode[] listOfDialogue;           // Array of all of the DialogueNodes that correspond to this event

        [Header("Sub UI Variables")]
        public TMP_ColorGradient selectChoiceHighlight; // Reference to the ColorGradiant used to showcase which text the player has selected

        // Private Variables
        private Transform choiceBoxUI;                  // Reference to the UI element that contains all of the choices

        private Animator dialogueWindowAnimator;        // Hard references to all of the UI elements stored for dialouges
        private Animator choiceWindowAnimator;
        private TextMeshProUGUI dialogueUI;
        private TextMeshProUGUI nameUI;
        private Image portaitUI;
        private Image proceedIconUI;

        private int currentDialogueId;                  // The ID of the current dialogue that is being used.
        private int currCharIndex;                      // The character index that AnimateText is currently on when displaying the text             
        private int currChoiceIndex;                    // The index number that is used to identify what choice the player selected
        private int numbOfAvailableChoices;             // Keeps track of the number of choices the player can select from

        private bool isDialoguePaused;                  // Is the dialogue currently paused?
        private bool hasShownDialogueText;              // Has this dialogue finished showing all of its text and events?
        private bool hasShownChoices;                   // Has this dialogue shown all of its choices, if necessary
        private bool hasFinishedEvents;                 // Has this dialogue finished all of its events, if applicable?

        // Caches the dialouge and image components for future use
		private void Start()
		{
            // IMPORTANT: The childs in here may change depending on the ordering of the GameObjects in the UI scene!
            portaitUI = objectToInteract.transform.GetChild(0).GetChild(0).GetComponent<Image>();
            dialogueUI = objectToInteract.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
            proceedIconUI = objectToInteract.transform.GetChild(0).GetChild(3).GetComponent<Image>();
            nameUI = objectToInteract.transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
            dialogueWindowAnimator = objectToInteract.transform.GetChild(0).GetComponent<Animator>();

            choiceBoxUI = objectToInteract.transform.GetChild(1).GetChild(1);
            choiceWindowAnimator = objectToInteract.transform.GetChild(1).GetComponent<Animator>();
		}

		// Displays the given text to the UI, if the event is activated
		private void Update()
		{
            if(hasActivated == true)
            {
                // Handles showing the proceed UI if it can be shown
                if(hasShownDialogueText == true && hasFinishedEvents == true && proceedIconUI.isActiveAndEnabled == false)
                {
                    proceedIconUI.enabled = true;
                }

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

                // As long as the dialogue window is shown, we can interact with it.
                if(Input.GetKeyDown(interactKey) && isDialoguePaused == false)
                {
                    if(hasShownChoices == true)
                    {
                        // We selected a choice and we are running it
                        StartCoroutine(SelectedChoice());
                    }
                    else if(hasFinishedEvents == true)
                    {
                        if(hasShownDialogueText == false)
                        {
                            // We skip the animation and show the entire text
                            currCharIndex = listOfDialogue[currentDialogueId].DialogueText.Length;
                        }
                        else if(listOfDialogue[currentDialogueId].CheckIfChildrenExist() == true)
                        {
                            // We set the next dialogue to use the first child of the current dialogue node
                            currentDialogueId = listOfDialogue[currentDialogueId].GetChildIdAtIndex(0);
                            PlayNextDialogue();
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
            isDialoguePaused = false;

            dialogueUI.text = "";
            nameUI.text = "";
            portaitUI.sprite = null;

            // We set the first dialogue to be the first item in the dialogue node list
            currentDialogueId = 0;
            PlayNextDialogue();
		}

        // When the dialogue is finished, we reset it
		public override void EventOutcome()
        {
            // If we have a cutscene associated with this dialogue, we do NOT reset this event or the game state
            // The reason is that the the cutscene itself will script when the cutscene is over
            // So unless we are a standalone dialogue, we do not reset the game state or this event
            if(endDialogueEvent.GetType() != typeof(TransitionArea))
            {
                // NOTE that if we have a transition area, we do NOT allow it to reset the game state
                if(associatedCutscene == null)
                {
                    GameManager.Instance.CurrentState = GameStates.NORMAL;
                    Invoke("ResetEvent", 0.5f);
                }
            }
            HideDialogueBox();
            isFinished = true;
        }

		// When called, will hide the dialogue box and pause the dialogue
		public void HideDialogueBox()
        {
            if(hasShownChoices == true)
            {
                choiceWindowAnimator.SetBool("isOpen", false);
            }
            dialogueWindowAnimator.SetBool("isOpen", false);

            if(hasActivated == true)
            {
                isDialoguePaused = true;
            }
        }

        // When called, will show the dialogue box and resume the dialogue
        public void ShowDialogueBox()
        {
            if(hasShownChoices == true)
            {
                choiceWindowAnimator.SetBool("isOpen", true);
            }
            dialogueWindowAnimator.SetBool("isOpen", true);

            if(hasActivated == true)
            {
                isDialoguePaused = false;
            }
        }

        // If we have choices, we fill them out with whatever dialogue node we are on.
        private void FillOptions()
        {
            if(listOfDialogue[currentDialogueId].CheckIfChildrenExist())
            {
                // We reset these two vars to 0, since we are filling in new options
                numbOfAvailableChoices = 0;
                currChoiceIndex = 0;

                int childIndex = 0;
                int currChildId = listOfDialogue[currentDialogueId].GetChildIdAtIndex(childIndex);
                while(currChildId != -1)
                {
                    // This check verifies if the current child ID is valid in the current dialogue
                    if(currChildId > 0 && currChildId < listOfDialogue.Length)
                    {
                        // If so, we fill in the UI with the choice that corresponds to that childID
                        choiceBoxUI.GetChild(numbOfAvailableChoices).GetComponent<TextMeshProUGUI>().text = listOfDialogue[currChildId].DialogueChoiceText;
                        choiceBoxUI.GetChild(numbOfAvailableChoices).gameObject.SetActive(true);
                        numbOfAvailableChoices++;
                    }
                    childIndex++;
                    currChildId = listOfDialogue[currentDialogueId].GetChildIdAtIndex(childIndex);
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
        }

        // Helper method that changes the two texts in the Choice Box at the specified indexes to change gradiants
        private void ChangeSelectedText(int oldIndex, int newIndex)
        {
            choiceBoxUI.GetChild(oldIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = null;
            choiceBoxUI.GetChild(newIndex).GetComponent<TextMeshProUGUI>().colorGradientPreset = selectChoiceHighlight;
        }

        // Helper method that starts up a dialogue box
        private void PlayNextDialogue()
        {
            if(associatedCutscene != null && listOfDialogue[currentDialogueId].GetDelayTime != 0)
            {
                // If there's a delay in the first dialogue node, we enact that
                // We also only do this if this dialogue is associated with a cutscene
                StartCoroutine(DelayDialogue());
            }
            else
            {
                // Else, we proceed to the next dialogue point
                if(currentDialogueId == 0)
                {
                    // If we are on the first dialogue, we always show up the dialogue box
                    ShowDialogueBox();
                }
                StartCoroutine(AnimateText());
                StartCoroutine(StartDialogueNodeEvent());
            }
        }

        // Animates the text to appear in a typewriter fashion!
        private IEnumerator AnimateText()
        {
            // We initialize the animation to start and fill in the name and portraits
            hasShownDialogueText = false;
            portaitUI.sprite = listOfDialogue[currentDialogueId].DialoguePortrait;
            nameUI.text = listOfDialogue[currentDialogueId].DialogueName;
            proceedIconUI.enabled = false;
            yield return null;

            // The actual place where the "animation" occues
            for(currCharIndex = 0; currCharIndex < listOfDialogue[currentDialogueId].DialogueText.Length; ++currCharIndex)
            {
                dialogueUI.text = listOfDialogue[currentDialogueId].DialogueText.Substring(0,currCharIndex);
                yield return new WaitForSeconds(typeWriteDelay);
            }
            // We then fill in the rest of the dialogue
            dialogueUI.text = listOfDialogue[currentDialogueId].DialogueText;
            yield return null;

            // If we have choices present, we activate them here
            if(listOfDialogue[currentDialogueId].CheckIfMultipleChilrenExist())
            {
                ClearOptions();
                FillOptions();
                yield return null;
            }

            // We then enable the rest of the window to appear
            hasShownDialogueText = true;
            yield return null;
        }
    
        // Handles the logic animation for selecting a choice
        private IEnumerator SelectedChoice()
        {
            // All of the choices are listed in the order that they are put in, meaning each choice slot directly corresponds to the 
            // Id slot in the children id array.
            // As such, we can use the currentChoiceIndex to get the approperiate child id to use to advance the dialogue
            currentDialogueId = listOfDialogue[currentDialogueId].GetChildIdAtIndex(currChoiceIndex);
            ClearOptions();
            yield return null;

            PlayNextDialogue();
            hasShownChoices = false;
            yield return null;
        }
    
        // Plays out an event if the specific dialogue node has one
        private IEnumerator StartDialogueNodeEvent()
        {
            if(listOfDialogue[currentDialogueId].CheckIfEventIsCompleted() == false)
            {
                hasFinishedEvents = false;
                listOfDialogue[currentDialogueId].ActivateEvent();
                while(listOfDialogue[currentDialogueId].DialogueEvent.IsFinished == false)
                {
                    yield return null;
                }
            }
            // Once the dialogue has finished playing out, we tell the Script that it can continue
            hasFinishedEvents = true;
            yield return null;
        }
    
        // Plays out the final event that the dialogue event has one
        // This event is seperate from the dialogue events, so this takes place after everything is completed
        private IEnumerator StartDialogueEventEnd()
        {
            if(endDialogueEvent.HasActivated == false)
            {
                HideDialogueBox();
                endDialogueEvent.gameObject.SetActive(true);
                while(endDialogueEvent.IsFinished == false)
                {
                    yield return null;
                }
            }
            EventOutcome();
        }
    
        // Delays the current dialogue from showing immediatly. Used in tangent with a cutscene
        private IEnumerator DelayDialogue()
        {
            // When the dialogue is delayed, we resume our cutscene
            HideDialogueBox();
            associatedCutscene.ChangeCutsceneState("Resume");
            yield return new WaitForSeconds(listOfDialogue[currentDialogueId].GetDelayTime);

            // When the time is up, we pause the cutscene
            associatedCutscene.ChangeCutsceneState("Pause");
            yield return null;

            // And then we show the dialogue options
            ShowDialogueBox();
            StartCoroutine(AnimateText());
            StartCoroutine(StartDialogueNodeEvent());
            yield return null;
        }
    }
}