/*  A data file that is used to store and templete useful information
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts 
{
    // This determines what kind of gear this is
    public enum GearType {
        WEAPON,
        ARMOR,
        ACCESSORY
    }

    [CreateAssetMenu(fileName = "New Gear Data", menuName = "Custom Data/Gear")]
    public class GearData : ScriptableObject {

        [Header("General Variables")]
        public GearType typeOfGear;
        public string gearName;
        public string gearDescription;

        [Header("Gear Specific")]
        [Tooltip("The values are granted toward a specific stat based on its index position in the array\nEx: [HP, SP, ATTACK, DEFENSE]")]
        /*  What kind of bonuses does this armor grant to the user?
         *  The values are granted toward a specific stat based on its index position in the array
         *  Ex: [HP, SP, ATTACK, DEFENSE]
         */
        [Range(0,999)]
        public int[] statModifiers = new int[4];

        // TODO: In the future, may add a new attribute that allows for bonus effects when equipping certain gear
        // This may need to be a data type
    }   
}
