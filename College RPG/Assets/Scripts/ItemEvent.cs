/*  A basic event that gives the player an item when activated
 */

using UnityEngine;

namespace MattScripts {

    public class ItemEvent : BaseEvent
    {
        [Header("Sub Variables")]
        public ScriptableObject itemToGive; // The object that this event will grant to the player
        [Range(-99,99)]
        public int quantity = 1;        // How much is granted/taken from the player?
        public bool canResetItself;

		// Private variables
		private PlayerInventory playerInventory;

        // Verfiies if the quantity is a valid number
        private void OnValidate()
        {
            if(quantity == 0)
            {
                Debug.LogWarning("Quantity cannot be 0! Setting it to 1...");
                quantity = 1;
            }
        }

        // Saves a reference of the player to the script
		public override void EventSetup()
		{
            playerInventory = GameManager.Instance.PlayerReference.GetComponent<PlayerInventory>();
            EventOutcome();
		}

		// Upon being enabled, this event will grant the player the specific object
		public override void EventOutcome()
        {
            if(itemToGive is ItemData)
            {
                // We add the new item to the player inventory.
                // Depending on the amount of said item, we will increment or decrement from the player's inventory.
                InventoryItem newItem = new InventoryItem((ItemData)itemToGive, quantity);
                if(Mathf.Sign(quantity) < 0)
                {
                    Debug.Log("Took away " + newItem.SpecifiedItem.itemName + " from player.");
                    playerInventory.RemoveItemFromInventory(newItem, quantity);
                }
                else
                {
                    Debug.Log("Gave player " + newItem.SpecifiedItem.itemName);
                    playerInventory.AddToInventory(newItem);
                }
                isFinished = true;
            }
            else if(itemToGive is GearData)
            {
                // We add the new gear to the inventory
                // Depending on the amount of said item, we will increment or decrement from the player's inventory.
                InventoryGear newGear = new InventoryGear((GearData)itemToGive, quantity);
                if(Mathf.Sign(quantity) < 0)
                {
                    Debug.Log("Took away " + newGear.SpecifiedGear.gearName + " from player.");
                    playerInventory.RemoveGearFromInventory(newGear, quantity);
                }
                else
                {
                    Debug.Log("Gave player " + newGear.SpecifiedGear.gearName);
                    playerInventory.AddToInventory(newGear);
                }
                isFinished = true;
            }
            else if(itemToGive is CharacterData)
            {
                // We add the new party member to the player's inventory
                InventoryParty newCharacter = new InventoryParty((CharacterData)itemToGive);
                Debug.Log("Added " + newCharacter.SpecifiedCharacter.characterName + " to party.");
                playerInventory.AddToInventory(newCharacter);
                isFinished = true;
            }
            else if(itemToGive is LinkData)
            {
                // We add the new link to the player's inventory
                InventoryLink newLink = new InventoryLink((LinkData)itemToGive);
                Debug.Log("Added " + newLink.SpecifiedLink.linkName + " to inventory.");
                playerInventory.AddToInventory(newLink);
                isFinished = true;
            }

            if(canResetItself == true)
            {
                Invoke("ResetEvent", 0.5f);
            }
        }

        // Upon activation, we play out this event
        // Once activated, this event will not be able to be replayed
        // And this will only activate if there is no activation area
		private void OnEnable()
		{
            if(hasActivated == false && activateArea == null)
            {
                EventSetup();
                EventOutcome();
                hasActivated = true;
            }
		}

        // Resets this event to be activated again
		public override void ResetEvent()
		{
            hasActivated = false;
            isFinished = false;
            gameObject.SetActive(false);
		}
	}   
}
