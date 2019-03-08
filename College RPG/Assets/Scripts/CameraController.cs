/* Controls how the camera moves with the player. 
 * Specifically, this allows the camera to be bound to certain cordinates, depending on what the passed in values are
 * 
 * References:
 * Compare Layer with a LayerMask : https://answers.unity.com/questions/422472/how-can-i-compare-colliders-layer-to-my-layermask.html
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class CameraController : MonoBehaviour {

        [Header("General Variables")]
        [HideInInspector]
        public Transform objectToFollow;
        public LayerMask solidSurfaceLayer;

        [Header("Positioning Variables")]
        public Vector3 cameraOffset;
        public float minXPos;
        public float maxXPos;
        public float minYPos;
        public float maxYPos;
        public float minZPos;
        public float maxZPos;
       
        // We update the position of the camera based on the player's movement
        private void LateUpdate()
        {
            if(objectToFollow != null)
            {
                // We apply the initial position to be where the player is and add the offset from the player
                Vector3 newPos = objectToFollow.transform.position + cameraOffset;

                // We then clamp the position to the min/max values we gathered
                gameObject.transform.position = new Vector3(Mathf.Clamp(newPos.x, minXPos, maxXPos), Mathf.Clamp(newPos.y, minYPos, maxYPos), Mathf.Clamp(newPos.z, minZPos, maxZPos));
            }
        }

        // If the camera moves into a wall, we make the wall transparent
		private void OnTriggerEnter(Collider other)
		{
            if(((1<<other.gameObject.layer) & solidSurfaceLayer) != 0)
            {
                MeshRenderer mr = other.gameObject.GetComponent<MeshRenderer>();

                // If the camera is too close to a wall, we make the wall nearly transparent
                Color newColor = new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, 0.1f);
                mr.material.color = newColor;

                // This is a helper method that changes the material type of the player so that transparency is acheived
                StandardShaderUtils.ChangeRenderMode(mr.material, StandardShaderUtils.BlendMode.Fade);
            }
		}

        // When the camera exits a wall, the wall becomes apparent again
		private void OnTriggerExit(Collider other)
		{
            if(((1<<other.gameObject.layer) & solidSurfaceLayer) != 0)
            {
                MeshRenderer mr = other.gameObject.GetComponent<MeshRenderer>();

                // We reset the wall's colors back to normal.
                Color newColor = new Color(mr.material.color.r, mr.material.color.g, mr.material.color.b, 1f);
                mr.material.color = newColor;

                // This is a helper method that changes the material type of the player so that transparency is acheived
                StandardShaderUtils.ChangeRenderMode(mr.material, StandardShaderUtils.BlendMode.Opaque);
            }
		}
    }
}


