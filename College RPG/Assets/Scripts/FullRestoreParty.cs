/*  When this event is activated, the party will be healed fully.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class FullRestoreParty : BaseEvent
    {
        //private variables
        private PlayerInventory playerInventory;

        // When this event is enabled, we play out its role.
        // Afterwards, we disable it again
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

        // Goes through the entire party and heals them to the max
		public override void EventOutcome()
        {
            for(int i = 0; i < playerInventory.GetPartyInvetorySize(); i++)
            {
                InventoryParty currMember = playerInventory.GetInventoryCharacterAtIndex(i);
                currMember.CurrentHealthPoints = currMember.ReturnModdedStat("MaxHP");
                currMember.CurrentSkillPoints = currMember.ReturnModdedStat("MaxSP");
                Debug.Log(currMember.SpecifiedCharacter.characterName + " healed to the max!");
            }
            GameManager.Instance.CurrentState = GameStates.NORMAL;
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

