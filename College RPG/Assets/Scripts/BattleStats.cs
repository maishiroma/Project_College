/*  Encapsulates important data into this class that is used in battles.
 *  Every character in a battle will have this script in order to have an easier way of organizing all of the entities
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {
    public class BattleStats : MonoBehaviour {

        public ScriptableObject battleData;         // A reference to the custom data that this object is using. VERY important.

        // Private variables
        private int currentHP;                      // These control values that will periodically change during the battle
        private int maxHP;

        private int currentSP;
        private int maxSP;

        /*  This Dictionary will have the following keys:
         *  MaxHP
         *  MaxSP
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

        // Getter/Setter for HP. Makes sure HP does not go below 0 or over the max amount
        public int CurrentHP {
            get { return currentHP; }
            set {
                if(value < 0)
                {
                    currentHP = 0;
                }
                else if(value > maxHP)
                {
                    currentHP = maxHP;
                }
                else
                {
                    currentHP = value;
                }
            }
        }

        // Getter/Setter for SP. Makes sure the SP does not go below 0 or over the max amount
        public int CurrentSP {
            get { return currentSP; }
            set {
                if(value < 0)
                {
                    currentSP = 0;
                }
                else if(value > maxSP)
                {
                    currentSP = maxSP;
                }
                else
                {
                    currentSP = value;
                }
            }
        }

        // We initialize the entity here
        // This is overloaded
        // This definition is used for enemies
        public void InitalizeEntity(int newHP, int newSP)
        {
            modifierValuesForStats = new Dictionary<string, int>();

            modifierValuesForStats.Add("MaxHP",0);
            modifierValuesForStats.Add("MaxSP", 0);
            modifierValuesForStats.Add("PhysicalAttack",0);
            modifierValuesForStats.Add("PhysicalDefense",0);
            modifierValuesForStats.Add("SpecialAttack",0);
            modifierValuesForStats.Add("SpecialDefense",0);
            modifierValuesForStats.Add("Speed",0);
            modifierValuesForStats.Add("Luck",0);
            keyArray = new string[modifierValuesForStats.Keys.Count];
            modifierValuesForStats.Keys.CopyTo(keyArray, 0);

            currentHP = newHP;
            currentSP = newSP;
            maxHP = currentHP;
            maxSP = currentSP;
        }
        // This definition is used for players
        public void InitalizeEntity(InventoryParty partyMember)
        {
            modifierValuesForStats = partyMember.GetModifierValuesForStats;
            keyArray = new string[modifierValuesForStats.Keys.Count];
            modifierValuesForStats.Keys.CopyTo(keyArray, 0);

            currentHP = partyMember.CurrentHealthPoints;
            currentSP = partyMember.CurrentSkillPoints;
            maxHP = ReturnModdedStat("MaxHP");
            maxSP = ReturnModdedStat("MaxSP");
        }

        // Using the passed in party member, we save the current HP/SP to the party member
        public void SaveCurrentHPSP(InventoryParty savedPartyMenber)
        {
            savedPartyMenber.CurrentHealthPoints = currentHP;
            savedPartyMenber.CurrentHealthPoints = currentSP;
        }

        // We compare the speeds of the called object and the other data that was passed in
        public bool CompareSpeeds(BattleStats other)
        {
            if(ReturnModdedStat("Speed") > other.ReturnModdedStat("Speed"))
            {
                return true;
            }
            else if(ReturnModdedStat("Speed") < other.ReturnModdedStat("Speed"))
            {
                return false;
            }
            else
            {
                // If we have a tie in speed, we will randomly decide which one will be faster
                return (Random.Range(0,2) == 1);
            }
        }    

        // Calculates how damage will be dealt to this entity
        // Returns the amount of damage done
        public int DealDamage(BattleStats attacker, AttackData currentAttack)
        {
            int attackPower = 0;
            int defensePower = 0;

            // If the attacker is the player, we are attacking an enemy
            if(attacker.battleData is CharacterData)
            {
                if(currentAttack.attackType == AttackType.PHYSICAL)
                {
                    // Because the attack is physical, we calculate the math based on physical power
                    attackPower = attacker.ReturnModdedStat("BaseAttack") + currentAttack.attackPower + attacker.ReturnModdedStat("PhysicalAttack");
                    defensePower = ReturnModdedStat("PhysicalDefense");
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    // Because the attack is special, we calculate the math based on special power
                    attackPower = attacker.ReturnModdedStat("BaseAttack") + currentAttack.attackPower + attacker.ReturnModdedStat("SpecialAttack");
                    defensePower = ReturnModdedStat("SpecialDefense");

                    // We also deduct SP from the user as well
                    attacker.CurrentSP -= currentAttack.attackCost;
                }
            }
            // If the attacker is an enemy, we are attacking a party member
            else if(attacker.battleData is EnemyData)
            {
                if(currentAttack.attackType == AttackType.PHYSICAL)
                {
                    // Because the attack is physical, we calculate the math based on physical power
                    attackPower = currentAttack.attackPower + attacker.ReturnModdedStat("PhysicalAttack");
                    defensePower = ReturnModdedStat("BaseDefense") + ReturnModdedStat("PhysicalDefense");
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    // Because the attack is special, we calculate the math based on special power
                    attackPower = currentAttack.attackPower + attacker.ReturnModdedStat("SpecialAttack");
                    defensePower = ReturnModdedStat("BaseDefense") + ReturnModdedStat("SpecialDefense");

                    // We also deduct SP from the user as well
                    attacker.CurrentSP -= currentAttack.attackCost;
                }
            }

            // Modifies the total attack power depending on the affinity of the attack
            switch(GetAttackEffectiveness(currentAttack))
            {
                case AffinityValues.RESISTANT:
                    attackPower /= 2;
                    break;
                case AffinityValues.WEAK:
                    attackPower *= 2;
                    break;
                case AffinityValues.NULL:
                    attackPower *= 0;
                    break;
            }

            // We then calculate the final damage, clamping the damage to 0 and infinity (cannot be negative)
            int finalDamage = (int)Mathf.Clamp(attackPower - defensePower, 0, Mathf.Infinity);
            currentHP -= finalDamage;

            // We then return the total damage output
            return finalDamage;
        }

        // Returns true if the entity can use this attack
        public bool CheckIfCanUseAttack(int attackIndex)
        {
            AttackData currentAttack = null;

            if(battleData is CharacterData)
            {
                currentAttack = ((CharacterData)battleData).demonData.attackList[attackIndex];
            }
            else if(battleData is EnemyData)
            {
                currentAttack = ((EnemyData)battleData).attackList[attackIndex];
            }

            // If we can use the attack, we return true. Else, we return false
            if(CurrentSP > currentAttack.attackCost)
            {
                return true;
            }
            return false;
        }

        // Returns the given affinity value that the attack has on the current entity
        public AffinityValues GetAttackEffectiveness(AttackData currentAttack)
        {
            if(battleData is CharacterData)
            {
                return ((CharacterData)battleData).demonData.affinityArray[ReturnAttackAffinityIndex(currentAttack)];
            }
            else if(battleData is EnemyData)
            {
                return ((EnemyData)battleData).affinityArray[ReturnAttackAffinityIndex(currentAttack)];
            }

            // By default, we will return normal effectiveness
            return AffinityValues.NORMAL;
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
            }
        }

        // Returns the value of a stat after it has been modded by the modifiers
        public int ReturnModdedStat(string statName)
        {
            if(battleData is CharacterData)
            {
                CharacterData moddedData = (CharacterData)battleData;
                switch(statName)
                {
                    case "MaxHP":
                        return moddedData.maxHealthPoints + modifierValuesForStats[statName];
                    case "MaxSP":
                        return moddedData.maxSkillPoints + modifierValuesForStats[statName];
                    case "BaseAttack":
                        return moddedData.baseAttack + modifierValuesForStats[statName];
                    case "BaseDefense":
                        return moddedData.baseDefense + modifierValuesForStats[statName];
                    case "PhysicalAttack":
                        return moddedData.demonData.phyAttackStat + modifierValuesForStats[statName];
                    case "PhysicalDefense":
                        return moddedData.demonData.phyDefenseStat + modifierValuesForStats[statName];
                    case "SpecialAttack":
                        return moddedData.demonData.spAttackStat + modifierValuesForStats[statName];
                    case "SpecialDefense":
                        return moddedData.demonData.spDefenseStat + modifierValuesForStats[statName];
                    case "Speed":
                        return moddedData.demonData.spdStat + modifierValuesForStats[statName];
                    case "Luck":
                        return moddedData.demonData.luckStat + modifierValuesForStats[statName];
                    default:
                        Debug.LogError(statName + " does not exist!");
                        return -1;
                }
            }
            else if(battleData is EnemyData)
            {
                EnemyData moddedData = (EnemyData)battleData;
                switch(statName)
                {
                    case "MaxHP":
                        return moddedData.maxHealthPoints + modifierValuesForStats[statName];
                    case "MaxSP":
                        return moddedData.maxSkillPoints + modifierValuesForStats[statName];
                    case "PhysicalAttack":
                        return moddedData.phyAttackStat + modifierValuesForStats[statName];
                    case "PhysicalDefense":
                        return moddedData.phyDefenseStat + modifierValuesForStats[statName];
                    case "SpecialAttack":
                        return moddedData.spAttackStat + modifierValuesForStats[statName];
                    case "SpecialDefense":
                        return moddedData.spDefenseStat + modifierValuesForStats[statName];
                    case "Speed":
                        return moddedData.spdStat + modifierValuesForStats[statName];
                    case "Luck":
                        return moddedData.luckStat + modifierValuesForStats[statName];
                    default:
                        Debug.LogError(statName + " does not exist!");
                        return -1;
                }
            }
            Debug.LogError("The data type does not exist!");
            return -1;
        }

        // Helper method that translates an attack's affinity to an index number
        // This index number corresponds to an entity's affiliated affinity with that affinity
        // Refer to the AttackData for a comprehensive list on the chart
        private int ReturnAttackAffinityIndex(AttackData currentAttack)
        {
            switch(currentAttack.attackAffinity)
            {
                case AttackAffinity.CONTACT:
                    return 0;
                case AttackAffinity.FIRE:
                    return 1;
                case AttackAffinity.ICE:
                    return 2;
                case AttackAffinity.WIND:
                    return 3;
                case AttackAffinity.THUNDER:
                    return 4;
                case AttackAffinity.LIGHT:
                    return 5;
                case AttackAffinity.DARK:
                    return 6;
                case AttackAffinity.ALMIGHTY:
                    return 7;
            }
            Debug.Log("The affinity on this attack does not exist!");
            return -1;
        }
    }
}
