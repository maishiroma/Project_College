/*  This script handles all of the player graphic components, including sprite changes
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class PlayerGraphics : MonoBehaviour {

        [Header("Component References")]
        public SpriteRenderer playerSprite;

        // Private Variables
        private GameObject mainCamera;

        // Gets the private variables initialized
        private void Awake()
        {
            mainCamera = GameObject.FindWithTag("MainCamera");
            if(mainCamera == null)
            {
                Debug.LogError("Cannot find Main Camera in Scene!");
            }
        }

        // Always has the sprite to look at the camera
        private void LateUpdate()
        {
            Vector3 newPos = mainCamera.transform.position - transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(newPos.x, 0, newPos.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.5f);
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
