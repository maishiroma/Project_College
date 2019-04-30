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
        public bool canReplayEvent = false;     // Can the cutscene be replayed upon completion?
        public bool canResetCamera = true;      // Does the camera reset to be near the player once the cutscene is over?

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

		// Stops the camera from following the player and sets up the cutscene visuals
		public override void EventSetup()
		{
            // We turn off the interactIconUI
            if(activateByInteract == true)
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

            // We then save the camera's last position and tell the GameManager that we are in a cutscene
            if(canResetCamera == true)
            {
                GameManager.Instance.MainCamera.SaveTransform();
            }
            GameManager.Instance.MainCamera.objectToFollow = null;
            GameManager.Instance.CurrentState = GameStates.EVENT;
		}

		// Resets the cutscene to go back to the normal game mode, if necessary
		public override void EventOutcome()
        {
            // Note that if the event can either be replayed or activated by interacting, the game will reset back to normal
            // If neither are true, when the cutscene ends, the game WILL NOT toggle back!
            // This is done because in special dialogue and cutscene scenes, we handle when the game is ready to go back.
            if(canReplayEvent == true)
            {
                ResetEvent();
                GameManager.Instance.CurrentState = GameStates.NORMAL;
            }
            if(activateByInteract == true)
            {
                interactIconUI.SetActive(true);
                GameManager.Instance.CurrentState = GameStates.NORMAL;
            }
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
            if(canResetCamera == true)
            {
                GameManager.Instance.MainCamera.LoadSavedTransform();
                GameManager.Instance.MainCamera.objectToFollow = GameManager.Instance.PlayerReference.transform;
            }
            isFinished = true;
            EventOutcome();
        }
    }
}