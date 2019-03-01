/*  This derives from BaseEvent, which will open up a dialogue box.
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
        public float typeWriteDelay;
        public DialogueNode[] listOfDialogue;

        [Header("Sub UI Variables")]
        public GameObject playerChoiceUI;

        // Private Variables
        private Transform choiceBoxUI;
        private DialogueNode currentDialogueNode;
        private Animator dialogueWindowUI;
        private TextMeshProUGUI dialogueUI;
        private TextMeshProUGUI nameUI;
        private Image portaitUI;
        private Image proceedIconUI;
        private int currAnimatedIndex;
        private bool isDisplayingText;
        private bool hasShownOptions;

        // Caches the dialouge and image components for future use
		private void Start()
		{
            // The childs in here may change depending on the ordering of the GameObjects in the UI scene
            portaitUI = objectToActivate.transform.GetChild(0).GetComponent<Image>();
            dialogueUI = objectToActivate.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            proceedIconUI = objectToActivate.transform.GetChild(3).GetComponent<Image>();
            nameUI = objectToActivate.transform.GetChild(4).GetChild(0).GetComponent<TextMeshProUGUI>();
            dialogueWindowUI = objectToActivate.GetComponent<Animator>();
            choiceBoxUI = playerChoiceUI.transform.GetChild(1);
		}

		// Displays the given text to the UI, if the event is activated
		private void Update()
		{
            if(HasActivated)
            {
                if(hasShownOptions == true)
                {
                    // TODO: Select the option that is currently highlighted
                }

                if(Input.GetKeyDown(interactKey))
                {
                    if(hasShownOptions == true)
                    {
                        // TODO: Need to fill in logic of selecting an option with W or S
                    }
                    else
                    {
                        if(isDisplayingText == true)
                        {
                            // We skip the animation and show the entire text
                            currAnimatedIndex = currentDialogueNode.dialogueText.Length;
                        }
                        else if(currentDialogueNode.CheckIfChildrenExist() == true)
                        {
                            // We proceed to the next dialogue point
                            currentDialogueNode = listOfDialogue[currentDialogueNode.GetFirstChild()];
                            StartCoroutine(AnimateText());
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
            interactIcon.SetActive(false);

            dialogueUI.text = "";
            nameUI.text = "";
            portaitUI.sprite = null;
            currentDialogueNode = listOfDialogue[0];

            // If we interact with this by going into it, we automatically start up the dialogue
            if(!activateByInteract)
            {
                StartCoroutine(AnimateText());
            }
            dialogueWindowUI.SetBool("isOpen", true);
		}

        // When the dialogue is finished, we reset it
		public override void EventOutcome()
        {
            GameManager.Instance.CurrentState = GameStates.NORMAL;
            dialogueWindowUI.SetBool("isOpen", false);
            Invoke("ResetEvent", 1f);
        }

        // If we have choices, we fill them out with whatever dialogue node we are on.
        private void FillOptions()
        {
            if(playerChoiceUI != null && currentDialogueNode.CheckIfChildrenExist())
            {
                int currIndexOfChoices = 0;
                foreach(int currChildId in currentDialogueNode.childrenNodeIds)
                {
                    if(currChildId != -1 && currChildId > 0 && currChildId < listOfDialogue.Length)
                    {
                        choiceBoxUI.GetChild(currIndexOfChoices).GetComponent<TextMeshProUGUI>().text = listOfDialogue[currChildId].choiceText;
                        choiceBoxUI.GetChild(currIndexOfChoices).gameObject.SetActive(true);
                        currIndexOfChoices++;
                    }
                }
                playerChoiceUI.SetActive(true);
                hasShownOptions = true;
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
            playerChoiceUI.SetActive(false);
            hasShownOptions = false;
        }

        // Animates the text to appear in a typewriter fashion!
        private IEnumerator AnimateText()
        {
            isDisplayingText = true;
            portaitUI.sprite = currentDialogueNode.portrait;
            nameUI.text = currentDialogueNode.nameText;

            proceedIconUI.enabled = false;
            yield return new WaitForEndOfFrame();

            for(currAnimatedIndex = 0; currAnimatedIndex < currentDialogueNode.dialogueText.Length; ++currAnimatedIndex)
            {
                dialogueUI.text = currentDialogueNode.dialogueText.Substring(0,currAnimatedIndex);
                yield return new WaitForSeconds(typeWriteDelay);
            }

            dialogueUI.text = currentDialogueNode.dialogueText;
            proceedIconUI.enabled = true;
            isDisplayingText = false;
            yield return null;

            if(currentDialogueNode.CheckIfMultipleChilrenExist())
            {
                ClearOptions();
                FillOptions();
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
