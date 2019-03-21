/*  A data file that is used to store and templete useful information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts 
{
    [CreateAssetMenu(fileName = "New Character Data", menuName = "Custom Data/Character")]
    public class CharacterData : ScriptableObject {

        [Header("Visuals")]
        public string characterName;

        [Header("Stats")]
        [Range(1,100)]
        public int characterLevel;

        [Space]
        [Range(0,9999)]
        public int currHealthPoints;
        [Range(0,9999)]
        public int maxHealthPoints;

        [Space]
        [Range(0,9999)]
        public int currSkillPoints;
        [Range(0,9999)]
        public int maxSkillPoints;

        [Space]
        [Range(1, 1000)]
        public int baseAttack;
        [Range(1, 1000)]
        public int baseDefense;

        [Header("External References")]
        public DemonData demonData;
        public List<AttackData> attackList = new List<AttackData>();
        public GearData[] equippedGear = new GearData[3];
    }   
}
