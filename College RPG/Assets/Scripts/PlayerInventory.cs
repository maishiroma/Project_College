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

    // The object that encapsulates gear in a list
    [System.Serializable]
    public class InventoryGear {
        public GearData specifiedGear;
        public int quantity = 0;            // How much of this one item is in the list?

        public InventoryGear(GearData newGear, int quantity)
        {
            this.specifiedGear = newGear;
            this.quantity = quantity;
        }
    }

    // The object that encapsulates characters in a list
    [System.Serializable]
    public class InventoryParty {
        public CharacterData specifiedCharacter;

        public int characterLevel = 1;    // All of these values are used in leveling up and maintaining a copy of the original data
        public int currentHealthPoints = 0;
        public int currentSkillPoints = 0;

        public InventoryParty(CharacterData newCharacter)
        {
            this.specifiedCharacter = newCharacter;

            this.currentHealthPoints = newCharacter.maxHealthPoints;
            this.currentSkillPoints = newCharacter.maxSkillPoints;
        }

        // TODO: Need to make a level up function that augments the player's stats.
        // TODO: Need to have a way to dynamically calculate the player's stats during battle.
    }

    // The object that encapsulates links in a list
    [System.Serializable]
    public class InventoryLink {
        public LinkData specifiedLink;

        public int linkLevel = 1;

        public InventoryLink(LinkData newLink)
        {
            this.specifiedLink = newLink;
        }

        // TODO: Need to make a function that levels up links
    }

    public class PlayerInventory : MonoBehaviour {

        // Private Variables
        private List<InventoryItem> listOfItems;
        private List<InventoryGear> listOfGear;
        private List<InventoryParty> listOfPartyMembers;
        private List<InventoryLink> listOfLinks;

        // Initialzes all of the private variables
		private void Start()
		{
            listOfItems = new List<InventoryItem>();
            listOfGear = new List<InventoryGear>();
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

        public void AddToInventory(InventoryGear newGear)
        {
            foreach(InventoryGear currGear in listOfGear)
            {
                // If this gear already exists in the list, we update the quantity
                if(currGear.specifiedGear.Equals(newGear.specifiedGear))
                {
                    currGear.quantity += newGear.quantity;
                    return;
                }
            }
            // If we haven't found an existing one, we add it to the list
            listOfGear.Add(newGear);
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
                    currItem.quantity -= quantity;
                    if(currItem.quantity < 0)
                    {
                        listOfItems.Remove(currItem);    
                    }
                    return true;
                }
            }
            return false;
        }

        // Removes a specified gear piece from the inventory using its index
        // Returns true if it was able to do this
        public bool RemoveGearFromInventory(int gearIndex, int quantity)
        {
            if(gearIndex < listOfItems.Count && gearIndex >= 0)
            {
                InventoryGear currGear = listOfGear[gearIndex];

                if(currGear.quantity - quantity < 0)
                {
                    listOfGear.Remove(currGear);
                }
                else
                {
                    currGear.quantity -= quantity;
                }
                return true;
            }
            return false;
        }

        // Like the aboe method, but this one takes a InventoryGear
        public bool RemoveGearFromInventory(InventoryGear specificGear, int quantity)
        {
            foreach(InventoryGear currGear in listOfGear)
            {
                // If this gear already exists in the list, we update the quantity
                if(currGear.specifiedGear.Equals(specificGear.specifiedGear))
                {
                    currGear.quantity -= quantity;
                    if(currGear.quantity < 0)
                    {
                        listOfGear.Remove(currGear);    
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

        // Returns the gearData at the specified index
        // returns null if it cannot find that gear
        public GearData GetGearAtIndex(int index)
        {
            if(index < listOfGear.Count && index >= 0)
            {
                return listOfGear[index].specifiedGear;
            }
            return null;
        }

        // Returns the character data at X index
        // Returns null if the index is invalid
        public CharacterData GetCharacterAtIndex(int index)
        {
            if(index < listOfPartyMembers.Count && index >= 0)
            {
                return listOfPartyMembers[index].specifiedCharacter;
            }
            return null;
        }

        // Returns the link data at X index
        // Returns null if the index is invalid
        public LinkData GetLinkAtIndex(int index)
        {
            if(index < listOfLinks.Count && index >= 0)
            {
                return listOfLinks[index].specifiedLink;
            }
            return null;
        }

        // Returns the size of the item inventory
        public int GetItemInventorySize()
        {
            return listOfItems.Count;
        }
	
        // Returns the size of the gear inventory
        public int GetGearInventorySize()
        {
            return listOfGear.Count;
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