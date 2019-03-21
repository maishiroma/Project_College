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
                    Debug.Log("Took away " + newItem.specifiedItem.itemName + " from player.");
                    playerInventory.RemoveItemFromInventory(newItem, quantity);
                }
                else
                {
                    Debug.Log("Gave player " + newItem.specifiedItem.itemName);
                    playerInventory.AddToInventory(newItem);
                }
            }
            else if(itemToGive is GearData)
            {
                // We add the new gear to the inventory
                InventoryGear newGear = new InventoryGear((GearData)itemToGive, quantity);
                if(Mathf.Sign(quantity) < 0)
                {
                    Debug.Log("Took away " + newGear.specifiedGear.gearName + " from player.");
                    playerInventory.RemoveGearFromInventory(newGear, quantity);
                }
                else
                {
                    Debug.Log("Gave player " + newGear.specifiedGear.gearName);
                    playerInventory.AddToInventory(newGear);
                }
            }
            else if(itemToGive is CharacterData)
            {
                // We add the new party member to the player's inventory
                InventoryParty newCharacter = new InventoryParty((CharacterData)itemToGive);
                Debug.Log("Added " + newCharacter.specifiedCharacter.characterName + " to party.");
                playerInventory.AddToInventory(newCharacter);
            }
            else if(itemToGive is LinkData)
            {
                // We add the new link to the player's inventory
                InventoryLink newLink = new InventoryLink((LinkData)itemToGive);
                Debug.Log("Added " + newLink.specifiedLink.linkName + " to inventory.");
                playerInventory.AddToInventory(newLink);
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
