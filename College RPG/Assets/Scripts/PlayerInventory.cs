/*  This script stores all of the data that the player has, which includes:
 *  items, skills, gear, team members, etc
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    // The object that encapsulates items in the list
    [System.Serializable]
    public class InventoryItem {
        public ItemData specifiedItem;
        public int quantity;            // How much of this one item is in the list?

        public InventoryItem(ItemData newItem, int quantity)
        {
            this.specifiedItem = newItem;
            this.quantity = quantity;
        }
    }

    // The object that encapsulates characters in a list
    [System.Serializable]
    public class InventoryParty {
        public CharacterData specifiedPartyMember;

        public InventoryParty(CharacterData newCharacter)
        {
            this.specifiedPartyMember = newCharacter;
        }
    }

    // The object that encapsulates links in a list
    [System.Serializable]
    public class InventoryLink {
        public LinkData specifiedLinkData;

        public InventoryLink(LinkData newLink)
        {
            this.specifiedLinkData = newLink;
        }
    }

    public class PlayerInventory : MonoBehaviour {

        // Private Variables
        private List<InventoryItem> listOfItems;
        private List<InventoryParty> listOfPartyMembers;
        private List<InventoryLink> listOfLinks;

        // Initialzes all of the private variables
		private void Start()
		{
            listOfItems = new List<InventoryItem>();
            listOfPartyMembers = new List<InventoryParty>();
            listOfLinks = new List<InventoryLink>();
		}

        // Adds an item to an inventory
        // This is overloaded so that it can properly add to the right list
        public void AddToInventory(InventoryItem newItem)
        {
            foreach(InventoryItem currItem in listOfItems)
            {
                // If this item already exists in the list, we update the quantity
                if(currItem.specifiedItem.Equals(newItem.specifiedItem))
                {
                    currItem.quantity += newItem.quantity;
                    return;
                }
            }
            // If we haven't found an existing one, we add it to the list
            listOfItems.Add(newItem);
        }

        public void AddToInventory(InventoryParty newCharacter)
        {
            if(!listOfPartyMembers.Contains(newCharacter))
            {
                listOfPartyMembers.Add(newCharacter);
            }
        }

        public void AddToInventory(InventoryLink newLink)
        {
            if(!listOfLinks.Contains(newLink))
            {
                listOfLinks.Add(newLink);
            }
        }

        // Removes a specific item from the list if it is known by the specific quantity
        // And returns it if it was successful, else, returns null
        public InventoryItem RemoveItemFromInventory(int itemIndex, int quantity)
        {
            if(itemIndex < listOfItems.Count && itemIndex >= 0)
            {
                InventoryItem currItem = listOfItems[itemIndex];

                if(currItem.quantity - quantity < 0)
                {
                    listOfItems.Remove(currItem);
                }
                else
                {
                    currItem.quantity -= quantity;
                }
                return currItem;
            }
            return null;
        }

        // This one is similar to the above method, but we return true/false if we were able to do the operation
        public bool RemoveItemFromInventory(InventoryItem specificItem, int quantity)
        {
            foreach(InventoryItem currItem in listOfItems)
            {
                // If this item already exists in the list, we update the quantity
                if(currItem.specifiedItem.Equals(specificItem.specifiedItem))
                {
                    specificItem.quantity -= quantity;
                    if(specificItem.quantity < 0)
                    {
                        listOfItems.Remove(specificItem);    
                    }
                    return true;
                }
            }
            return false;
        }

        // Returns the itemdata at the specified index point
        // Returns null if it cannot find that item
        public ItemData GetItemAtIndex(int index)
        {
            if(index < listOfItems.Count && index >= 0)
            {
                return listOfItems[index].specifiedItem;
            }
            return null;
        }

        // Returns the character data at X index
        // Returns null if the index is invalid
        public CharacterData GetCharacterAtIndex(int index)
        {
            if(index < listOfPartyMembers.Count && index >= 0)
            {
                return listOfPartyMembers[index].specifiedPartyMember;
            }
            return null;
        }

        // Returns the link data at X index
        // Returns null if the index is invalid
        public LinkData GetLinkAtIndex(int index)
        {
            if(index < listOfLinks.Count && index >= 0)
            {
                return listOfLinks[index].specifiedLinkData;
            }
            return null;
        }

        // Returns the size of the item inventory
        public int GetItemInventorySize()
        {
            return listOfItems.Count;
        }
	
        // Returns the size of the party member list
        public int GetPartyInvetorySize()
        {
            return listOfPartyMembers.Count;
        }

        // Returns the size of the link inventory
        public int GetLinkInventorySize()
        {
            return listOfLinks.Count;
        }
    }
}