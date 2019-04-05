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
    }

}
