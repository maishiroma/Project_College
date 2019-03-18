/*  A data file that is used to store and templete useful information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts 
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Custom Data/Enemy")]
    public class EnemyData : ScriptableObject {

        [Header("Visuals")]
        public Sprite enemySprite;

        [Space]
        public string enemyName;

        [Header("Stats")]
        [Range(1,100)]
        public int enemyLevel;

        [Space]
        [Range(1,15000)]
        public int currHealthPoints;
        [Range(1,15000)]
        public int maxHealthPoints;

        [Space]
        [Range(1,15000)]
        public int currSkillPoints;
        [Range(1,15000)]
        public int maxSkillPoints;

        [Space]
        [Range(1,9999)]
        public int phyAttackStat;
        [Range(1,9999)]
        public int phyDefenseStat;

        [Space]
        [Range(1,9999)]
        public int spAttackStat;
        [Range(1,9999)]
        public int spDefenseStat;

        [Space]
        [Range(1,9999)]
        public int spdStat;
        [Range(1,9999)]
        public int luckStat;

        [Header("Affinities")]
        /* Keeps track of all of the weaknesses/resistances has
        * These are index based, using the order that the AttackAffinity enums are declared
        * so the index are mapped innatly to these mappings in here
        * 
        * 0: CONTACT
        * 1: FIRE
        * 2: ICE
        * 3: WIND
        * 4: THUNDER
        * 5: LIGHT
        * 6: DARK
        * 7: ALMIGHTY
        */
        public AffinityValues[] affinityArray = new AffinityValues[] 
        {   
            AffinityValues.NORMAL, 
            AffinityValues.NORMAL, 
            AffinityValues.NORMAL, 
            AffinityValues.NORMAL, 
            AffinityValues.NORMAL, 
            AffinityValues.NORMAL, 
            AffinityValues.NORMAL
        };

        [Header("External References")]
        public List<AttackData> attackList = new List<AttackData>();
    }   
}
