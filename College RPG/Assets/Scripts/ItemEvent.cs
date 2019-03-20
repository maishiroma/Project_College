/*  A basic event that gives the player an item when activated
 */

using UnityEngine;

namespace MattScripts {

    public class ItemEvent : BaseEvent
    {
        [Header("Sub Variables")]
        public ScriptableObject itemToGive;

        [Range(-99,99)]
        public int quantity = 1;        // How much is granted/taken from the player?

        // Verfiies if the quantity is a valid number
		private void OnValidate()
		{
            if(quantity == 0)
            {
                Debug.LogWarning("Quantity cannot be 0! Setting it to 1...");
                quantity = 1;
            }
		}

		// Private variables
		private PlayerInventory playerInventory;

        // Saves a reference of the player to the script
		public override void EventSetup()
		{
            playerInventory = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();
		}

		// Upon being enabled, this event will grant the player the specific object
		public override void EventOutcome()
        {
            // TODO: When we have other item types, we need to play out the specific cast and methods
            if(itemToGive is ItemData)
            {
                InventoryItem newItem = new InventoryItem((ItemData)itemToGive, quantity);

                if(Mathf.Sign(quantity) < 0)
                {
                    Debug.Log("Took away " + ((ItemData)(itemToGive)).itemName + " from player.");
                    playerInventory.RemoveItemFromInventory(newItem, quantity);
                }
                else
                {
                    Debug.Log("Gave player" + ((ItemData)(itemToGive)).itemName);
                    playerInventory.AddToInventory(newItem);
                }
            }
        }

        // Upon activation, we play out this event
		private void OnEnable()
		{
            EventSetup();
            EventOutcome();
            hasActivated = true;
		}
	}   
}
