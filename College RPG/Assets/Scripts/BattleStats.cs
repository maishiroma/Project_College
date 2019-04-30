/*  Encapsulates important data into this class that is used in battles.
 *  Every character in a battle will have this script in order to have an easier way of organizing all of the entities
 */

using UnityEngine;

namespace MattScripts {
    public class BattleStats : MonoBehaviour {

        public ScriptableObject battleData;         // A reference to the custom data that this object is using. VERY important.

        // Private variables
        private int currentHP;                      // These control values that will periodically change during the battle
        private int maxHP;

        private int currentSP;
        private int maxSP;

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

        // We initialize both HP/SP to what is passed
        // This is an overloaded method
        public void InitializeHPSP(int newHP, int newSP)
        {
            currentHP = newHP;
            currentSP = newSP;
            maxHP = newHP;
            maxSP = newHP;
        }
        public void InitializeHPSP(int newHP, int newSP, int newMaxHP, int newMaxSP)
        {
            currentHP = newHP;
            currentSP = newSP;
            maxHP = newMaxHP;
            maxSP = newMaxSP;
        }

        // Using the passed in party member, we save the current HP/SP to the party member
        public void SaveCurrentHPSP(InventoryParty savedPartyMenber)
        {
            savedPartyMenber.CurrentHealthPoints = currentHP;
            savedPartyMenber.CurrentHealthPoints = currentSP;
        }

        // We compare the speeds of the called object and the other data that was passed in
        // This is an overloaded method
        public bool CompareSpeeds(CharacterData other)
        {
            if(battleData is CharacterData)
            {
                CharacterData caller = (CharacterData)battleData;

                if(caller.demonData.spdStat > other.demonData.spdStat)
                {
                    return true;
                }
                else if(caller.demonData.spdStat < other.demonData.spdStat)
                {
                    return false;
                }
                else
                {
                    // If we have a tie in speed, we will randomly decide which one will be faster
                    return (Random.Range(0,2) == 1);
                }
            }
            else if(battleData is EnemyData)
            {
                EnemyData caller = (EnemyData)battleData;

                if(caller.spdStat > other.demonData.spdStat)
                {
                    return true;
                }
                else if(caller.spdStat < other.demonData.spdStat)
                {
                    return false;
                }
                else
                {
                    return (Random.Range(0,2) == 1);
                }
            }
            else
            {
                Debug.LogError("You did not pass a valid object in battle data!");
                return false;
            }
        }    
        public bool CompareSpeeds(EnemyData other)
        {
            if(battleData is CharacterData)
            {
                CharacterData caller = (CharacterData)battleData;

                if(caller.demonData.spdStat > other.spdStat)
                {
                    return true;
                }
                else if(caller.demonData.spdStat < other.spdStat)
                {
                    return false;
                }
                else
                {
                    // If we have a tie in speed, we will randomly decide which one will be faster
                    return (Random.Range(0,2) == 1);
                }
            }
            else if(battleData is EnemyData)
            {
                EnemyData caller = (EnemyData)battleData;

                if(caller.spdStat > other.spdStat)
                {
                    return true;
                }
                else if(caller.spdStat < other.spdStat)
                {
                    return false;
                }
                else
                {
                    // If we have a tie in speed, we will randomly decide which one will be faster
                    return (Random.Range(0,2) == 1);
                }
            }
            else
            {
                Debug.LogError("You did not pass a valid object in battle data!");
                return false;
            }
         }

        // Calculates how damage will be dealt to this entity
        // Returns the amount of damage done
        public int DealDamage(BattleStats attacker, AttackData currentAttack)
        {
            int attackPower = 0;
            int defensePower = 0;

            if(attacker.battleData is CharacterData)
            {
                // If the attacker is a party member, we are attacking an enemy
                CharacterData attacker_casted = (CharacterData)attacker.battleData;
                EnemyData target = (EnemyData)battleData;

                if(currentAttack.attackType == AttackType.PHYSICAL)
                {
                    // Because the attack is physical, we calculate the math based on physical power
                    attackPower = attacker_casted.baseAttack + currentAttack.attackPower + attacker_casted.demonData.phyAttackStat;
                    defensePower = target.phyDefenseStat;
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    // Because the attack is special, we calculate the math based on special power
                    attackPower = attacker_casted.baseAttack + currentAttack.attackPower + attacker_casted.demonData.spAttackStat;
                    defensePower = target.spDefenseStat;

                    // We also deduct SP from the user as well
                    attacker.CurrentSP -= currentAttack.attackCost;
                }
                Debug.Log(attacker_casted.characterName + " attacked " + target.enemyName);
            }
            else if(attacker.battleData is EnemyData)
            {
                // If the attacker is an enemy, we are attacking a party member
                EnemyData attacker_casted = (EnemyData)attacker.battleData;
                CharacterData target = (CharacterData)battleData;

                if(currentAttack.attackType == AttackType.PHYSICAL)
                {
                    // Because the attack is physical, we calculate the math based on physical power
                    attackPower = currentAttack.attackPower + attacker_casted.phyAttackStat;
                    defensePower = target.baseDefense + target.demonData.phyDefenseStat;
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    // Because the attack is special, we calculate the math based on special power
                    attackPower = currentAttack.attackPower + attacker_casted.spAttackStat;
                    defensePower = target.baseDefense + target.demonData.phyDefenseStat;

                    // We also deduct SP from the user as well
                    attacker.CurrentSP -= currentAttack.attackCost;
                }
                Debug.Log(attacker_casted.enemyName + " attacked " + target.characterName);
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
            Debug.Log("The attack did " + finalDamage + " damage.");

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
