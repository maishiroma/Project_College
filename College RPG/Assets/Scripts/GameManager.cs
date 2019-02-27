/*  This controls game wide functions
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MattScripts {

    // These determine what the game is currently doing, and each state will invoke different actions accordingly
    public enum GameStates {
        NORMAL,
        EVENT
    }

    public class GameManager : MonoBehaviour {

        // Static variables
        public static GameManager Instance;

        // Private Variables
        [SerializeField]
        private GameStates currentState;
        private GameObject player;

        // Getters/Setters
        public GameStates CurrentState{
            get {return currentState;}
            set {
                currentState = value;

                // When the game is in a certain state, we disable the player's controller
                switch(currentState)
                {
                    case GameStates.NORMAL:
                        player.GetComponent<CharacterController>().EnableController();
                        break;
                    case GameStates.EVENT:
                        player.GetComponent<CharacterController>().DisableController();
                        break;
                }
            }
        }

        // Singleton GameObject; There will only be one instance of this at a given time.
		private void Awake()
		{
            if(Instance == null)
            {
                Instance = gameObject.GetComponent<GameManager>();
                DontDestroyOnLoad(Instance);
            }
            else
            {
                Destroy(gameObject);
            }
		}

        // Initializes private variables
		private void Start()
		{
            player = GameObject.FindWithTag("Player");
		}
	}
}