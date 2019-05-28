/*  Upon activation, this event will cause the player party to take damage
 * 
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class FieldDamage : BaseEvent
    {
        [Header("Sub Variable")]
        public int subtractor;

        //private variables
        private PlayerInventory playerInventory;

        // Makes the class variable always positive
		private void OnValidate()
		{
            subtractor = Mathf.Abs(subtractor);
		}

		// When this object is activated, this script will take place
		private void OnEnable()
        {
            if(playerInventory == null)
            {
                playerInventory = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();
            }

            if(hasActivated == false)
            {
                hasActivated = true;
                EventOutcome();
                Invoke("ResetEvent", 1f);
            }
        }

		// Goes through the entire party and damages them by the specified amount
		public override void EventOutcome()
        {
            for(int i = 0; i < playerInventory.GetPartyInvetorySize(); i++)
            {
                InventoryParty currMember = playerInventory.GetInventoryCharacterAtIndex(i);
                currMember.CurrentHealthPoints -= subtractor;
                Debug.Log(currMember.SpecifiedCharacter.characterName + " took " + subtractor + " damage!");
            }
            isFinished = true;
        }

        // Resets this event to be activable again
        public override void ResetEvent()
        {
            if(hasActivated == true)
            {
                hasActivated = false;
                isFinished = false;
                gameObject.SetActive(false);
            }
        }
    }
}