/*  This script handles all of the player graphic components, including sprite changes
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class PlayerGraphics : MonoBehaviour {

        [Header("External References")]
        public SpriteRenderer playerSprite;

        // Always has the sprite to look at the camera
        private void LateUpdate()
        {
            if(GameManager.Instance.MainCamera != null)
            {
                Vector3 newPos = GameManager.Instance.MainCamera.transform.position - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(newPos.x, 0, newPos.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.5f);
            }
        }

        // Changes the sprite to whatever action is passed into here.
        public void ChangeSprite(string stateName)
        {
            switch(stateName)
            {
                default:    // Prints out error if a state is misspelled or it doesn't exist
                    print("I don't know what " + stateName + " is!");
                    break;
            }
        }
    }
}
