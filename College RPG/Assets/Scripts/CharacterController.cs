/*  This defines how the player moves. Very simple.
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    public class CharacterController : MonoBehaviour {

        [Header("General Variables")]
        public float forwardVelocity;               // How fast does the player move?
        public float rotateVelocity;                // How fast does the player rotate?
        [Range(0.01f, 5f)]
        public float groundedDisitance;           // The disitance the character needs to be in in order to be considered grounded
        [Range(0.1f, 1f)]
        public float gravityAcceleration;          // How fast will we move downward when we are not grounded

        public LayerMask solidWallLayer;            // The Layer used to mark solid walls

        [Header("Input Variables")]
        public string FORWARD_AXIS;                 // These next three indicate the buttons used to control the character
        public string TURN_AXIS;

        // Private variables
        [Space]
        [SerializeField]
        private Rigidbody playerRB;
        private Quaternion targetRotation;
        private Vector3 targetVelocity;
        private float forwardInput;
        private float turnInput;

        // Turns off the controller. Should be used instead of calling .enabled, since this turns off multiple aspects to the controller
        public void DisableController()
        {
            playerRB.velocity = Vector3.zero;
            enabled = false;
        }

        // Reactivates the controller. Should be used instead of calling .enabled
        public void EnableController()
        {
            enabled = true;
        }

        // Returns true if the character is grounded (using a Raycast? Should be called as few times as possible)
        public bool GetGrounded()
        {
            if(Physics.Raycast(gameObject.transform.position, Vector3.down, groundedDisitance, solidWallLayer))
            {
                return true;
            }
            return false;
        }

        // Initializes default values for the private variables
        private void Start()
        {
            forwardInput = 0;
            targetVelocity = Vector3.zero;
            targetRotation = gameObject.transform.rotation;
            if(gameObject.GetComponent<Rigidbody>() == null)
            {
                Debug.LogError("You forgot to add a Rigidbody to the character!");
            }
        }

        // Used for non physics update
        private void Update()
        {
            GetInput();
            Turn();
        }

        // Used for physics updates
        private void FixedUpdate()
        {
            Run();
            VerticalMovement();
            playerRB.velocity = gameObject.transform.TransformDirection(targetVelocity);
        }

        // Gets the player Input and used in the Update
        private void GetInput()
        {
            forwardInput = Input.GetAxis(FORWARD_AXIS);
            turnInput = Input.GetAxis(TURN_AXIS);
        }

        // Moves the player
        private void Run()
        {
            if(forwardInput < 0)
            {
                // If the player is moving backwards, they can move
                targetVelocity.z = forwardVelocity * forwardInput;
            }
            else if(forwardInput > 0 && Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, 0.5f, solidWallLayer) == false)
            {
                // If the player is moving forward AND there is no wall in front of them, they can move
                targetVelocity.z = forwardVelocity * forwardInput;
            }
            else
            {
                // The player stops moving
                targetVelocity.z = 0;
            }
        }

        // Turns the player
        private void Turn()
        {
            // We reorientate the player acccording to how they turn.
            // In order to add the new rotation to the current rotation, we need to use multiplication
            // We also do the turn effect in here
            targetRotation *= Quaternion.AngleAxis(rotateVelocity * turnInput * Time.deltaTime, Vector3.up);
            gameObject.transform.rotation = targetRotation;
        }
    
        // Moves the player upward, downward, or staying on the ground.
        private void VerticalMovement()
        {
            bool isGrounded = GetGrounded();
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
    }
}