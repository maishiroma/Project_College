/*  This class defines all core functionalities for every events in the game and will have various useful functions on
 *  how the game handles these.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public abstract class BaseEvent : MonoBehaviour {

        [Header("Base Variables")]
        public bool activateByInteract;
        public bool showTriggerArea;
        public KeyCode interactKey;

        [Header("Base Outside Refs")]     
        public BoxCollider activateArea;
        public GameObject interactIconUI;
        public GameObject objectToInteract;

        // Protected Variables
        protected bool hasActivated;        // Set to true when the event is triggered
        protected bool isFinished;          // Set to true when the event is done

        // Getters
        public bool HasActivated {
            get {return hasActivated;}
        }
        public bool IsFinished {
            get {return isFinished;}
        }

		// When the player first enters the area, the event will occur accordingly
		private void OnTriggerEnter(Collider other)
		{
            if(other.CompareTag("Player") && hasActivated == false && GameManager.Instance.CurrentState != GameStates.MENU)
            {
                if(activateByInteract)
                {
                    // Display HUD icon to show that the player can interact with this.
                    interactIconUI.SetActive(true);
                }
                else
                {
                    // Start the event automatically
                    objectToInteract.SetActive(true);
                    EventSetup();
                    hasActivated = true;
                }
            }
		}

		// When the player is in this area, the event will trigger accordingly
		private void OnTriggerStay(Collider other)
		{
            if(other.CompareTag("Player") && hasActivated == false)
            {
                // If we are in the menu, we do NOT allow for the player to active the event
                if(GameManager.Instance.CurrentState != GameStates.MENU)
                {
                    // If the player is within the trigger still, but the UI hasn't activated, we make sure we activate it
                    if(activateByInteract == true && interactIconUI.activeInHierarchy == false)
                    {
                        interactIconUI.SetActive(true);
                    }

                    if(activateByInteract && Input.GetKeyDown(interactKey))
                    {
                        // Check if the player has hit the approperiate button and starts the event accordingly
                        objectToInteract.SetActive(true);
                        EventSetup();
                        hasActivated = true;
                    }
                }
                else
                {
                    // We turn off the interact icon if we are in the menu
                    if(activateByInteract == true && interactIconUI.activeInHierarchy == true)
                    {
                        interactIconUI.SetActive(false);
                    }
                }
            }
		}

        // When the player leaves the area, the event will react accordingly
		private void OnTriggerExit(Collider other)
		{
            if(other.CompareTag("Player") && hasActivated == false)
            {
                if(activateByInteract == true)
                {
                    // Remove the HUD for interacting with this
                    interactIconUI.SetActive(false);
                }
            }
		}

        // When selected and when enabled, allows for the hitbox to be visualized
		private void OnDrawGizmos()
		{
            if(showTriggerArea == true)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(activateArea.gameObject.transform.position, activateArea.size);
            }
		}

		// Resets this event back to its default state.
		// Can be overwritten in sub classes
		public virtual void ResetEvent()
        {
            if(hasActivated == true)
            {
                interactIconUI.SetActive(false);
                objectToInteract.SetActive(false);
                hasActivated = false;
                isFinished = false;
            }
        }
	
        // If needed, sends additional information to the activated event
        // Can be overriden in sub classes
        public virtual void EventSetup()
        {
            // Right now, all this does is just returns, aka, does nothing
            return;
        }

        // Called when the event is finished
        // Must be defined in all subclasses.
        public abstract void EventOutcome();
    }
}

