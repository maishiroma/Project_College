/*  A simple funtion that removes an object from the scene after X seconds
 *  Once done,this event will destroy this gameobject
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class TimedRemovalEvent : BaseEvent {

        [Header("Sub Variables")]
        public bool shouldDeactivate;
        [Range(0.1f, 10f)]
        public float timeToRemove;

		// Starts up the behavior of this Script to delete the specified GameObject after X seconds
		private void OnEnable()
        {
            Invoke("EventOutcome", timeToRemove);
        }

        // Resets this Script and restarts its removal period
        // Only works if the GameObject is not destroyed
		public override void ResetEvent()
		{
            if(shouldDeactivate == true && objectToInteract != null && hasActivated == true)
            {
                objectToInteract.SetActive(true);
                base.ResetEvent();
            }
		}

        // Removes or destroys the gameObject
		public override void EventOutcome()
        {
            if(shouldDeactivate)
            {
                hasActivated = true;
                objectToInteract.SetActive(false);
            }
            else
            {
                Destroy(objectToInteract);
            }
        }
    }

}

