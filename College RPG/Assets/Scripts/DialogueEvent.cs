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
        [TextArea(1, 4)]
        public string[] listOfDialogue;
        public Sprite[] listOfPortraits;

        // Private Variables
        private TextMeshProUGUI dialogueUI;
        private Image portaitUI;
        private Image proceedIconUI;
        private int currDialogIndex;
        private int currAnimatedIndex;
        private bool isDisplayingText;

        // Checks to see if the length of the portraits are the same as the dialogue boxes
		private void Start()
		{
            if(listOfDialogue.Length != listOfPortraits.Length)
            {
                Debug.LogError("The portait length must be the same as the dialogue length!");
            }
		}

		// Displays the given text to the UI, if the event is activated
		private void Update()
		{
            if(HasActivated)
            {
                if(Input.GetKeyDown(interactKey))
                {
                    if(isDisplayingText == true)
                    {
                        // We skip the animation and show the entire text
                        currAnimatedIndex = listOfDialogue[currDialogIndex].Length;
                    }
                    else if(currDialogIndex + 1 < listOfDialogue.Length)
                    {
                        // We proceed to the next dialogue point
                        currDialogIndex++;                       
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

        // Sets up important values that will be needed to be set before the event starts
		public override void EventSetup()
		{
            GameManager.Instance.CurrentState = GameStates.EVENT;
            interactIcon.SetActive(false);

            // We cache the dialouge and image components for future use
            // The childs in here may change depending on the ordering of the GameObjects in the UI scene
            portaitUI = objectToActivate.transform.GetChild(0).GetComponent<Image>();
            dialogueUI = objectToActivate.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            proceedIconUI = objectToActivate.transform.GetChild(3).GetComponent<Image>();
            dialogueUI.text = "";
            currDialogIndex = -1;

            // If we interact with this by going into it, we automatically start up the dialogue
            if(!activateByInteract)
            {
                currDialogIndex++;                       
                StartCoroutine(AnimateText());
            }
		}

        // When the dialogue is finished, we reset it
		public override void EventOutcome()
        {
            GameManager.Instance.CurrentState = GameStates.NORMAL;
            ResetEvent();
        }
    
        // Animates the text to appear in a typewriter fashion!
        private IEnumerator AnimateText()
        {
            isDisplayingText = true;
            portaitUI.sprite = listOfPortraits[currDialogIndex];
            proceedIconUI.enabled = false;
            yield return new WaitForEndOfFrame();

            for(currAnimatedIndex = 0; currAnimatedIndex < listOfDialogue[currDialogIndex].Length; ++currAnimatedIndex)
            {
                dialogueUI.text = listOfDialogue[currDialogIndex].Substring(0,currAnimatedIndex);
                yield return new WaitForSeconds(typeWriteDelay);
            }

            dialogueUI.text = listOfDialogue[currDialogIndex];
            proceedIconUI.enabled = true;
            isDisplayingText = false;
            yield return null;
        }
    }
}
