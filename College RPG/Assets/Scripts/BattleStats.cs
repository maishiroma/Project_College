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
        private int currentSP;

        // We initialize both HP/SP to what is passed
        public void InitializeHPSP(int newHP, int newSP)
        {
            currentHP = newHP;
            currentSP = newSP;
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
    }

}
