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
        [Range(0,9999)]
        public int maxHealthPoints;
        [Range(0,9999)]
        public int maxSkillPoints;

        [Space]
        [Range(1, 1000)]
        public int baseAttack;
        [Range(1, 1000)]
        public int baseDefense;

        [Header("External References")]
        [Tooltip("The associated Demon with this Character. This will augment and base the character's stats, affinities, and attacks.")]
        public DemonData demonData;

        [Tooltip("An array of all of the gear that the player has. These will also augment the character's stats.")]
        public GearData[] equippedGear = new GearData[3];
    }   
}
