/*  This derives from BaseEvent, which will open up a dialogue box.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MattScripts {

    public class DialogueEvent : BaseEvent
    {
        [Header("Sub Variables")]
        [TextArea(1, 4)]
        public string[] listOfDialogue;

        // Private Variables
        private TextMeshProUGUI dialogueUI;
        private int currDialogIndex;

        // Displays the given text to the UI, if the event is activated
		private void Update()
		{
            if(HasActivated)
            {
                if(Input.GetKeyDown(interactKey))
                {
                    if(currDialogIndex + 1 < listOfDialogue.Length)
                    {
                        currDialogIndex++;
                        dialogueUI.text = listOfDialogue[currDialogIndex];
                    }
                    else
                    {
                        EventOutcome();
                    }
                }
            }
		}

        // We cache the dialogueUI in this call before we begin the update
		public override void EventSetup()
		{
            interactIcon.SetActive(false);
            GameManager.Instance.CurrentState = GameStates.EVENT;
            currDialogIndex = -1;
            dialogueUI = objectToActivate.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
		}

        // When the dialogue is finished, we reset it
		public override void EventOutcome()
        {
            currDialogIndex = -1;
            ResetEvent();
            GameManager.Instance.CurrentState = GameStates.NORMAL;
            interactIcon.SetActive(true);
        }
    }
}
