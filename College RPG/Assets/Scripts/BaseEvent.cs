/*  This class defines the basis for events in the game and will have various useful functions on
 *  how the game handles these events.
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
        public GameObject objectToActivate;
        public GameObject interactIcon;

        // Private Variables
        private bool hasActivated;

        // Getters
        public bool HasActivated {
            get {return hasActivated;}
        }

		// When the player first enters the area, the event will occur accordingly
		private void OnTriggerEnter(Collider other)
		{
            if(other.CompareTag("Player") && hasActivated == false)
            {
                if(activateByInteract)
                {
                    // Display HUD icon to show that the player can interact with this.
                    interactIcon.SetActive(true);
                }
                else
                {
                    // Start the event automatically
                    objectToActivate.SetActive(true);
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
                if(activateByInteract && Input.GetKeyDown(interactKey))
                {
                    // Check if the player has hit the approperiate button and starts the event accordingly
                    objectToActivate.SetActive(true);
                    EventSetup();
                    hasActivated = true;
                }
            }
		}

        // When the player leaves the area, the event will react accordingly
		private void OnTriggerExit(Collider other)
		{
            if(other.CompareTag("Player") && hasActivated == false)
            {
                if(activateByInteract)
                {
                    // Remove the HUD for interacting with this
                    interactIcon.SetActive(false);
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
                interactIcon.SetActive(false);
                objectToActivate.SetActive(false);
                hasActivated = false;
            }
        }
	
        // If needed, sends additional information to the activated event
        // Can be overriden in sub classes
        public virtual void EventSetup()
        {
            // Right now, all this does is just returns, aka, does nothing
            return;
        }

        // Every class that inherits from this will have to define how each event will end.
        public abstract void EventOutcome();
    }
}

