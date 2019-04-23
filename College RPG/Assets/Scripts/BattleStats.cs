/*  Encapsulates important data into this class that is used in battles.
 *  Every character in a battle will have this script
 *  Also contains useful methods to call on
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {
    public class BattleStats : MonoBehaviour {

        public ScriptableObject battleData;

        // Private variables
        private int currentHP;
        private int maxHP;
        private int currentSP;
        private int maxSP;
        private bool isDead;

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

        public bool IsDead {
            get { return isDead;}
            set {
                if(currentHP <= 0)
                {
                    isDead = true;
                }
                else
                {
                    isDead = value;
                }
            }
        }


        // We initialize both HP/SP to what is passed
        public void InitializeHPSP(int newHP, int newSP, int newMaxHP, int newMaxSP)
        {
            currentHP = newHP;
            currentSP = newSP;
            maxHP = newMaxHP;
            maxSP = newMaxSP;
        }
        // This is an overloaded method that takes in the current HP/SP and sets those to the max
        public void InitializeHPSP(int newHP, int newSP)
        {
            currentHP = newHP;
            currentSP = newSP;
            maxHP = newHP;
            maxSP = newHP;
        }

        // We compare the speeds of the two datas that are passed in
        // Returns true if the object calling this is faster than other
        public bool CompareSpeeds(BattleStats other)
        {
            if(battleData is CharacterData)
            {
                if(other.battleData is CharacterData)
                {
                    if(((CharacterData)battleData).demonData.spdStat > ((CharacterData)other.battleData).demonData.spdStat)
                    {
                        return true;
                    }
                    else if(((CharacterData)battleData).demonData.spdStat < ((CharacterData)other.battleData).demonData.spdStat)
                    {
                        return false;
                    }
                    else
                    {
                        return (Random.Range(0,1) == 1);
                    }
                }
                else if(other.battleData is EnemyData)
                {
                    if(((CharacterData)battleData).demonData.spdStat > ((EnemyData)other.battleData).spdStat)
                    {
                        return true;
                    }
                    else if(((CharacterData)battleData).demonData.spdStat < ((EnemyData)other.battleData).spdStat)
                    {
                        return false;
                    }
                    else
                    {
                        return (Random.Range(0,1) == 1);
                    }
                }
                else
                {
                    Debug.LogError("You did not pass a valid object in other data!");
                    return false;
                }
            }
            else if(battleData is EnemyData)
            {
                if(other.battleData is CharacterData)
                {
                    if(((EnemyData)battleData).spdStat > ((CharacterData)other.battleData).demonData.spdStat)
                    {
                        return true;
                    }
                    else if(((EnemyData)battleData).spdStat < ((CharacterData)other.battleData).demonData.spdStat)
                    {
                        return false;
                    }
                    else
                    {
                        return (Random.Range(0,1) == 1);
                    }
                }
                else if(other.battleData is EnemyData)
                {
                    if(((EnemyData)battleData).spdStat > ((EnemyData)other.battleData).spdStat)
                    {
                        return true;
                    }
                    else if(((EnemyData)battleData).spdStat < ((EnemyData)other.battleData).spdStat)
                    {
                        return false;
                    }
                    else
                    {
                        return (Random.Range(0,1) == 1);
                    }
                }
                else
                {
                    Debug.LogError("You did not pass a valid object in battle data!");
                    return false;
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
                    attackPower = attacker_casted.baseAttack + currentAttack.attackPower + attacker_casted.demonData.phyAttackStat;
                    defensePower = target.phyDefenseStat;
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    attackPower = attacker_casted.baseAttack + currentAttack.attackPower + attacker_casted.demonData.spAttackStat;
                    defensePower = target.spDefenseStat;

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
                    attackPower = currentAttack.attackPower + attacker_casted.phyAttackStat;
                    defensePower = target.baseDefense + target.demonData.phyDefenseStat;
                }
                else if(currentAttack.attackType == AttackType.SPECIAL)
                {
                    attackPower = currentAttack.attackPower + attacker_casted.spAttackStat;
                    defensePower = target.baseDefense + target.demonData.phyDefenseStat;

                    attacker.CurrentSP -= currentAttack.attackCost;
                }
                Debug.Log(attacker_casted.enemyName + " attacked " + target.characterName);
            }

            // Modifies the attack power depending on the affinity of the attack
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

            int finalDamage = (int)Mathf.Clamp(attackPower - defensePower, 0, Mathf.Infinity);
            Debug.Log("The attack did " + finalDamage + " damage.");

            currentHP -= finalDamage;
            return finalDamage;
        }

        // Check if the player is dead
        public bool CheckIfDead()
        {
            if(currentHP <= 0)
            {
                // One of the player characters is dead, so we deactivate them
                // For now, we hide the enemy sprite
                isDead = true;
                gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;

                // TODO: Do some animation of the character dying
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
            return AffinityValues.NORMAL;
        }

        // Helper method that translates an attack's affinity to an index number
        // This index number corresponds to an entity's affiliated affinity with that affinity
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
