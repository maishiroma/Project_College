/*  A data file that is used to store and templete useful information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts 
{
    // What kind of attack is this? Determines if attk/def, spAttk/spDef is used
    public enum AttackType {
        PHYSICAL,
        SPECIAL,
        STATUS
    }

    // What Element is this attack associated with?
    public enum AttackAffinity {
        CONTACT,
        FIRE,
        ICE,
        WIND,
        THUNDER,
        LIGHT,
        DARK,
        ALMIGHTY
    }

    // Lists out all of the potental ways an attack can be received
    public enum AffinityValues {
        NORMAL,
        RESISTANT,
        NULL,
        ABSORB,
        REFLECT
    }

    [CreateAssetMenu(fileName = "New Attack Data", menuName = "Custom Data/Attack")]
    public class AttackData : ScriptableObject {

        [Header("Visual Variables")]
        public string attackName;

        [Space]
        [TextArea(1,3)]
        public string attackDescription;

        [Tooltip("The animation that plays out when this move is performed.")]
        public Animation attackAnimation;

        [Header("Attack Properties")]
        public AttackType attackType;
        public AttackAffinity attackAffinity;

        [Space]
        [Range(0,9999)]
        [Tooltip("How much HP/SP does this attack use up?")]
        public int attackCost;
        [Range(0,9999)]
        [Tooltip("The base power of this attack.")]
        public int attackPower;
    }   
}
