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
        public bool canResetCamera = true;

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

		// Stops the camera from following the player and sets up any useful methods needed
		public override void EventSetup()
		{
            if(activateByInteract)
            {
                interactIconUI.SetActive(false);
            }

            // Dynamically sets the player to be used in the animation if there exists a track named "Player"
            foreach(PlayableBinding curr in cutsceneObject.playableAsset.outputs){
                if(curr.sourceObject.name == "Player")
                {
                    cutsceneObject.SetGenericBinding(curr.sourceObject, GameManager.Instance.PlayerReference.GetComponent<Animator>());
                    break;
                }
            }

            GameManager.Instance.MainCamera.SaveTransform();
            GameManager.Instance.MainCamera.objectToFollow = null;
            GameManager.Instance.CurrentState = GameStates.EVENT;
		}

		// Resets the main camera to follow the player again.
		public override void EventOutcome()
        {
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

        // Changes the state of the cutscene if it can
        // Can etiehr accept: Resume, Pause, Stop
        public void ChangeCutsceneState(string state)
        {
            switch(state)
            {
                case "Resume":
                    if(cutsceneObject.state == PlayState.Paused)
                    {
                        cutsceneObject.Resume();
                    }
                    break;
                case "Pause":
                    if(cutsceneObject.state == PlayState.Playing)
                    {
                        cutsceneObject.Pause();
                    }
                    break;
                case "Stop":
                    if(cutsceneObject.state == PlayState.Playing)
                    {
                        cutsceneObject.Stop();
                    }
                    break;
                default:
                    Debug.LogError("I don't know what " + state + " is!");
                    break;
            }
        }

        // Delegate Event that is called when the cutscene is completed
        private void OnCutsceneComplete(PlayableDirector aDirector)
        {
            if(canResetCamera)
            {
                GameManager.Instance.MainCamera.RevertToOrigTransform();
                GameManager.Instance.MainCamera.objectToFollow = GameManager.Instance.PlayerReference.transform;
            }
            EventOutcome();
        }
    }
}