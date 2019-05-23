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
        private ItemData specifiedItem;
        private int quantity;            // How much of this one item is in the list?

        public ItemData SpecifiedItem {
            get {return specifiedItem;}
        }

        public int Quantity {
            get {return quantity;}
            set {
                if(value < 0)
                {
                    quantity = 0;
                }
                else
                {
                    quantity = value;
                }
            }
        }

        public InventoryItem(ItemData newItem, int quantity)
        {
            this.specifiedItem = newItem;
            this.quantity = quantity;
        }
	}

    // The object that encapsulates gear in a list
    [System.Serializable]
    public class InventoryGear {
        private GearData specifiedGear;
        private int quantity = 0;            // How much of this one item is in the list?

        public GearData SpecifiedGear {
            get {return specifiedGear;}
        }

        public int Quantity {
            get {return quantity;}
            set {
                if(value < 0)
                {
                    quantity = 0;
                }
                else
                {
                    quantity = value;
                }
            }
        }

        public InventoryGear(GearData newGear, int quantity)
        {
            this.specifiedGear = newGear;
            this.quantity = quantity;
        }
    }

    // The object that encapsulates characters in a list
    [System.Serializable]
    public class InventoryParty {
        private CharacterData specifiedCharacter;
        private int characterLevel = 1;             // All of these values are used in leveling up and maintaining a copy of the original data
        private int currentHealthPoints = 0;
        private int currentSkillPoints = 0;
        private int currentEXP = 0;
        private int toNextLevel = 100;

        /*  This Dictionary will have the following keys:
         *  HP
         *  SP
         *  BaseAttack (character data only)
         *  BaseDefense (character data only)
         *  PhysicalAttack
         *  PhysicalDefense
         *  SpecialAttack
         *  SpecialDefense
         *  Speed
         *  Luck
         */
        private Dictionary<string, int> modifierValuesForStats;       // This data structure holds all of the value increments for each of the stats an entiry has
        private string[] keyArray;                                    // An array of all of the keys that are available to be used

        public CharacterData SpecifiedCharacter {
            get {return specifiedCharacter;}
        }

        public int CharacterLevel {
            get {return characterLevel;}
            set {
                if(value > 0 && value < 101)
                {
                    characterLevel = value;
                }
            }
        }

        public int CurrentHealthPoints {
            get {return currentHealthPoints;}
            set {
                if(value < 0)
                {
                    currentHealthPoints = 0;
                }
                else if(value > specifiedCharacter.maxHealthPoints)
                {
                    currentHealthPoints = specifiedCharacter.maxHealthPoints;
                }
                else
                {
                    currentHealthPoints = value;
                }
            }
        }

        public int CurrentSkillPoints {
            get {return currentSkillPoints;}
            set {
                if(value < 0)
                {
                    currentSkillPoints = 0;
                }
                else if(value > specifiedCharacter.maxSkillPoints)
                {
                    currentSkillPoints = specifiedCharacter.maxSkillPoints;
                }
                else
                {
                    currentSkillPoints = value;
                }
            }
        }

        public int CurrentEXP {
            get { return currentEXP; }
            set {
                if(value > 0)
                {
                    currentEXP = value;
                }
            }
        }

        public int CurrentToNextLevel {
            get { return toNextLevel; }
            set {
                toNextLevel = value;
            }
        }

        public Dictionary<string, int> GetModifierValuesForStats {
            get { return modifierValuesForStats; }
        }

        // Initializes the party member
        public InventoryParty(CharacterData newCharacter)
        {
            this.specifiedCharacter = newCharacter;
            this.currentHealthPoints = newCharacter.maxHealthPoints;
            this.currentSkillPoints = newCharacter.maxSkillPoints;
            modifierValuesForStats = new Dictionary<string, int>();

            modifierValuesForStats.Add("MaxHP",0);
            modifierValuesForStats.Add("MaxSP", 0);
            modifierValuesForStats.Add("BaseAttack",0);
            modifierValuesForStats.Add("BaseDefense",0);
            modifierValuesForStats.Add("PhysicalAttack",0);
            modifierValuesForStats.Add("PhysicalDefense",0);
            modifierValuesForStats.Add("SpecialAttack",0);
            modifierValuesForStats.Add("SpecialDefense",0);
            modifierValuesForStats.Add("Speed",0);
            modifierValuesForStats.Add("Luck",0);

            keyArray = new string[modifierValuesForStats.Keys.Count];
            modifierValuesForStats.Keys.CopyTo(keyArray, 0);
        }
    
        // Method that increments up the values of an entity randomly
        public void LevelUpStats()
        {
            int numbStatsLeveledUp = Random.Range(2,modifierValuesForStats.Count);
            while(numbStatsLeveledUp > 0)
            {
                string randomStat = keyArray[Random.Range(0, keyArray.Length)];
                int randomIncrement = Random.Range(1,4);

                modifierValuesForStats[randomStat] += randomIncrement;
                numbStatsLeveledUp--;
                Debug.Log(randomStat + " went up by " + randomIncrement);
            }
        }
    
        // Helper method that returns a stat that is passed by Key to return the stat that incorperates the modifier
        // If the stat does not exist, returns -1
        public int ReturnModdedStat(string statName)
        {
            switch(statName)
            {
                case "MaxHP":
                    return specifiedCharacter.maxHealthPoints + modifierValuesForStats[statName];
                case "MaxSP":
                    return specifiedCharacter.maxSkillPoints + modifierValuesForStats[statName];
                case "BaseAttack":
                    return specifiedCharacter.baseAttack + modifierValuesForStats[statName];
                case "BaseDefense":
                    return specifiedCharacter.baseDefense + modifierValuesForStats[statName];
                case "PhysicalAttack":
                    return specifiedCharacter.demonData.phyAttackStat + modifierValuesForStats[statName];
                case "PhysicalDefense":
                    return specifiedCharacter.demonData.phyDefenseStat + modifierValuesForStats[statName];
                case "SpecialAttack":
                    return specifiedCharacter.demonData.spAttackStat + modifierValuesForStats[statName];
                case "SpecialDefense":
                    return specifiedCharacter.demonData.spDefenseStat + modifierValuesForStats[statName];;
                case "Speed":
                    return specifiedCharacter.demonData.spdStat + modifierValuesForStats[statName];;
                case "Luck":
                    return specifiedCharacter.demonData.luckStat + modifierValuesForStats[statName];;
                default:
                    Debug.LogError(statName + " does not exist!");
                    return -1;
            }
        }
    }

    // The object that encapsulates links in a list
    [System.Serializable]
    public class InventoryLink {
        private LinkData specifiedLink;
        private int linkLevel = 1;

        public LinkData SpecifiedLink {
            get {return specifiedLink;}
        }

        public int LinkLevel {
            get {return linkLevel;}
            set {
                if(value > 0 && value < 10)
                {
                    linkLevel = value;
                }
            }
        }

        public InventoryLink(LinkData newLink)
        {
            this.specifiedLink = newLink;
        }

        // TODO: Need to make a function that levels up links
    }

    public class PlayerInventory : MonoBehaviour {

        // Private Variables
        private int currentGold;
        private List<InventoryItem> listOfItems;
        private List<InventoryGear> listOfGear;
        private List<InventoryParty> listOfPartyMembers;
        private List<InventoryLink> listOfLinks;

        public int CurrentGold {
            get { return currentGold; }
            set {
                if(value > 0)
                {
                    currentGold = value;
                }
            }
        }

        // Initialzes all of the private variables
		private void Start()
		{
            listOfItems = new List<InventoryItem>();
            listOfGear = new List<InventoryGear>();
            listOfPartyMembers = new List<InventoryParty>();
            listOfLinks = new List<InventoryLink>();

            // TODO: Make sure to add in one value for the party inventory representing the player
		}

        // Adds an item to an inventory
        // This is overloaded so that it can properly add to the right list
        public void AddToInventory(InventoryItem newItem)
        {
            foreach(InventoryItem currItem in listOfItems)
            {
                // If this item already exists in the list, we update the quantity
                if(currItem.SpecifiedItem.Equals(newItem.SpecifiedItem))
                {
                    currItem.Quantity += newItem.Quantity;
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
                if(currGear.SpecifiedGear.Equals(newGear.SpecifiedGear))
                {
                    currGear.Quantity += newGear.Quantity;
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

                if(currItem.Quantity - quantity <= 0)
                {
                    listOfItems.Remove(currItem);
                }
                else
                {
                    currItem.Quantity -= quantity;
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
                if(currItem.SpecifiedItem.Equals(specificItem.SpecifiedItem))
                {
                    currItem.Quantity -= quantity;
                    if(currItem.Quantity <= 0)
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

                if(currGear.Quantity - quantity < 0)
                {
                    listOfGear.Remove(currGear);
                }
                else
                {
                    currGear.Quantity -= quantity;
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
                if(currGear.SpecifiedGear.Equals(specificGear.SpecifiedGear))
                {
                    currGear.Quantity -= quantity;
                    if(currGear.Quantity <= 0)
                    {
                        listOfGear.Remove(currGear);    
                    }
                    return true;
                }
            }
            return false;
        }

        // Returns the item at the specified index point
        // Returns null if it cannot find that item
        public InventoryItem GetItemAtIndex(int index)
        {
            if(index < listOfItems.Count && index >= 0)
            {
                return listOfItems[index];
            }
            return null;
        }

        // Returns the gear at the specified index
        // returns null if it cannot find that gear
        public InventoryGear GetGearAtIndex(int index)
        {
            if(index < listOfGear.Count && index >= 0)
            {
                return listOfGear[index];
            }
            return null;
        }

        // Returns the character at the specified index
        // returns null if it cannot find that character
        public InventoryParty GetInventoryCharacterAtIndex(int index)
        {
            if(index < listOfPartyMembers.Count && index >= 0)
            {
                return listOfPartyMembers[index];
            }
            return null;
        }

        // Returns the link at X index
        // Returns null if the index is invalid
        public InventoryLink GetLinkAtIndex(int index)
        {
            if(index < listOfLinks.Count && index >= 0)
            {
                return listOfLinks[index];
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