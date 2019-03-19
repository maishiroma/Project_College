/*  This script stores all of the data that the player has, which includes:
 *  items, skills, gear, team members, etc
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {
    
    public class PlayerInventory : MonoBehaviour {

        // Private Variables
        private List<ItemData> listOfItems;

        // Initialzes all of the private variables
		private void Start()
		{
            listOfItems = new List<ItemData>();
		}

        // Adds an item to the inventory
        // TODO: This will be an OVERLOADED method
        public void AddToInventory(ItemData newItem)
        {
            if(newItem != null)
            {
                listOfItems.Add(newItem);
            }
        }

        // Removes and returns an item from the inventory
        // If it fails, we return null
        // TODO: This will be an overloaded method
        public ItemData PopFromInventory(int itemIndex)
        {
            ItemData item = GetItemDataAtIndex(itemIndex);
            if(item != null)
            {
                listOfItems.RemoveAt(itemIndex);
                return item;
            }
            return null;
        }

        // Removes a specific item from the list if it is known
        // Returns true if it was successful
        // TODO: This will be an overloaded method
        public bool RemoveFromInventory(ItemData specificItem)
        {
            if(specificItem != null)
            {
                if(listOfItems.Contains(specificItem))
                {
                    listOfItems.Remove(specificItem);
                    return true;
                }
            }
            return false;
        }

        // Returns the item data at the specified index. Returns null if the index spot does not exist.
        public ItemData GetItemDataAtIndex(int index)
        {
            if(index < listOfItems.Count && index >= 0)
            {
                return listOfItems[index];
            }
            return null;
        }

        // Returns the size of the inventory
        public int GetInventorySize()
        {
            return listOfItems.Count;
        }
	}
}