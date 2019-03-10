/*  This script handles doing cutscenes in the game.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MattScripts {

    public class CutSceneEvent : BaseEvent
    {
        [Header("Sub Variables")]
        public bool canReplayEvent = false;

        // Private Variables
        private PlayableDirector cutsceneObject;

        // These two methods are used to set up the cutscene delegate
		private void OnEnable()
		{
            cutsceneObject.stopped += OnCutsceneComplete;
		}
		private void OnDisable()
		{
            cutsceneObject.stopped -= OnCutsceneComplete;
		}

        // Sets up all of the private variables
		private void Awake()
		{
            cutsceneObject = objectToInteract.GetComponent<PlayableDirector>();
		}

		// Stops the camera from following the player
		public override void EventSetup()
		{
            if(activateByInteract)
            {
                interactIconUI.SetActive(false);
            }
            GameManager.Instance.MainCamera.SaveTransform();
            GameManager.Instance.MainCamera.objectToFollow = null;
            GameManager.Instance.CurrentState = GameStates.EVENT;
		}

		// Resets the main camera to follow the player again.
		public override void EventOutcome()
        {
            GameManager.Instance.MainCamera.objectToFollow = GameManager.Instance.PlayerReference.transform;
            if(canReplayEvent == true)
            {
                ResetEvent();
            }
            if(activateByInteract)
            {
                interactIconUI.SetActive(true);
            }
            GameManager.Instance.CurrentState = GameStates.NORMAL;
        }

        // Delegate Event that is called when the cutscene is completed
        private void OnCutsceneComplete(PlayableDirector aDirector)
        {
            GameManager.Instance.MainCamera.RevertToOrigTransform();
            EventOutcome();
        }
    }
}