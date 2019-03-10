/*  This defines how the player moves. Very simple.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class CharacterController : MonoBehaviour {

        [Header("General Variables")]
        public float moveSpeed;                     // How fast does the player move?
        [Range(0.01f, 5f)]
        public float groundedDisitance;             // The disitance the character needs to be in in order to be considered grounded
        [Range(0.1f, 1f)]
        public float gravityAcceleration;           // How fast will we move downward when we are not grounded
        public LayerMask solidWallLayer;            // The Layer used to mark solid walls

        [Header("Input Variables")]
        public string UPDOWN_AXIS;                  // These next two indicate the buttons used to control the character
        public string LEFTRIGHT_AXIS;

        [Header("External Variables")]
        public GameObject frontOfPlayer;

        // Private variables
        [SerializeField]
        private Rigidbody playerRB;
        private Vector3 targetVelocity;
        private float verticalInput;
        private float horizontalInput;
        private bool isGrounded;

        // Turns off the controller. Should be used instead of calling .enabled, since this turns off multiple aspects to the controller
        public void DisableController()
        {
            playerRB.velocity = Vector3.zero;
            playerRB.isKinematic = true;
            enabled = false;
        }

        // Reactivates the controller. Should be used instead of calling .enabled
        public void EnableController()
        {
            verticalInput = 0;
            horizontalInput = 0;
            playerRB.isKinematic = false;
            enabled = true;
        }

        // We warp the player to the new position
        // We only do this if this is disabled
        public void WarpPlayer(Vector3 newPos)
        {
            if(isActiveAndEnabled == false)
            {
                playerRB.position = newPos;
            }
        }

        // Initializes default values for the private variables
        private void Start()
        {
            horizontalInput = 0;
            verticalInput = 0;
            targetVelocity = Vector3.zero;

            if(gameObject.GetComponent<Rigidbody>() == null)
            {
                Debug.LogError("You forgot to add a Rigidbody to the character!");
            }
        }

        // Used for non physics update
        private void Update()
        {
            GetInput();
        }

        // Used for physics updates
        private void FixedUpdate()
        {
            CheckIfGrounded();
            MovePlayer();
            VerticalMovement();

            // This reorientates the gameobject that is representing the front of the player
            if(targetVelocity.normalized.magnitude > 0.5f && isGrounded == true)
            {
                frontOfPlayer.transform.localPosition = targetVelocity.normalized / 2f;
            }
            playerRB.velocity = gameObject.transform.TransformDirection(targetVelocity);
        }

        // Gets the player Input and used in the Update
        private void GetInput()
        {
            verticalInput = Input.GetAxis(UPDOWN_AXIS);
            horizontalInput = Input.GetAxis(LEFTRIGHT_AXIS);
        }

        // Moves the player around the world
        private void MovePlayer()
        {
            targetVelocity.z = moveSpeed * verticalInput;
            targetVelocity.x = moveSpeed * horizontalInput;
        }
    
        // Moves the player upward, downward, or staying on the ground.
        private void VerticalMovement()
        {
            if(isGrounded == true)
            {
                // The player is grounded
                targetVelocity.y = 0;
            }
            else if(isGrounded == false)
            {
                // The player is falling
                targetVelocity.y -= gravityAcceleration;
            }
            else
            {
                // default case
                targetVelocity.y = 0;
            }
        }
    
        // Checks if the player is grounded and updated isGrounded
        private void CheckIfGrounded()
        {
            if(Physics.Raycast(gameObject.transform.position, Vector3.down, groundedDisitance, solidWallLayer))
            {
                isGrounded = true;
            }
            else
            {
                isGrounded = false;
            }
        }
    }
}