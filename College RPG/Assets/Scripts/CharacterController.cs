/*  This defines how the character moves. Very simple.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class CharacterController : MonoBehaviour {

        [Header("General Variables")]
        public float moveSpeed;                     // How fast does the character move?
        [Range(0.01f, 5f)]
        public float groundedDisitance;             // The disitance the character needs to be in in order to be considered grounded
        [Range(0.1f, 1f)]
        public float gravityAcceleration;           // How fast will we move downward when we are not grounded
        public LayerMask solidWallLayer;            // The Layer used to mark solid walls

        [Header("Input Variables")]
        public string UPDOWN_AXIS;                  // These next two indicate the buttons used to control the character
        public string LEFTRIGHT_AXIS;

        [Header("External Variables")]
        public GameObject frontOfCharacter;         // Visualizes the front of the player aka, the direction the player is facing

        // Private variables
        private Rigidbody characterRb;
        private Vector3 targetVelocity;
        private float verticalInput;
        private float horizontalInput;
        private bool isGrounded;

        // Turns off the controller. Should be used instead of calling .enabled, since this turns off multiple aspects to the controller
        public void DisableController()
        {
            if(enabled == true)
            {
                characterRb.velocity = Vector3.zero;
                characterRb.isKinematic = true;
                enabled = false;
            }
        }

        // Reactivates the controller. Should be used instead of calling .enabled
        public void EnableController()
        {
            if(enabled == false)
            {
                verticalInput = 0;
                horizontalInput = 0;
                characterRb.isKinematic = false;
                enabled = true;
            }
        }

        // We warp the character to the new position
        // We only do this if this is disabled
        public void WarpCharacter(Vector3 newPos)
        {
            if(enabled == false)
            {
                characterRb.position = newPos;
            }
        }

        // Initializes default values for the private variables
        private void Start()
        {
            horizontalInput = 0;
            verticalInput = 0;
            targetVelocity = Vector3.zero;
            characterRb = GetComponent<Rigidbody>();
        }

        // Used for non physics update
        // In this case, it is for input gets
        private void Update()
        {
            GetInput();
        }

        // Used for physics updates
        private void FixedUpdate()
        {
            CheckIfGrounded();
            HorizontalMovement();
            VerticalMovement();

            // This reorientates the frontOfCharacter gameobject
            if(targetVelocity.normalized.magnitude > 0.5f && isGrounded == true)
            {
                frontOfCharacter.transform.localPosition = targetVelocity.normalized / 2f;
            }
            characterRb.velocity = gameObject.transform.TransformDirection(targetVelocity);
        }

        // Gets the character Input and
        private void GetInput()
        {
            verticalInput = Input.GetAxis(UPDOWN_AXIS);
            horizontalInput = Input.GetAxis(LEFTRIGHT_AXIS);
        }

        // Moves the character around
        private void HorizontalMovement()
        {
            targetVelocity.z = moveSpeed * verticalInput;
            targetVelocity.x = moveSpeed * horizontalInput;
        }
    
        // Moves the character upward, downward, or staying on the ground.
        private void VerticalMovement()
        {
            if(isGrounded == true)
            {
                // The character is grounded
                targetVelocity.y = 0;
            }
            else if(isGrounded == false)
            {
                // The character is falling
                targetVelocity.y -= gravityAcceleration;
            }
            else
            {
                // default case
                targetVelocity.y = 0;
            }
        }
    
        // Checks if the character is grounded and updates isGrounded
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