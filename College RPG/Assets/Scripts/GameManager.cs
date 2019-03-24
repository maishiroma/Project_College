/*  This controls game wide functions
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MattScripts {

    // These determine what the game is currently doing, and each state will invoke different actions accordingly
    public enum GameStates {
        NORMAL,
        EVENT,
        TRAVEL,
        MENU
    }

    public class GameManager : MonoBehaviour {
        
        [Header("Prefab References")]
        public GameObject playerPrefab;

        // Static variables
        public static GameManager Instance;

        // Private Variables
        [SerializeField]
        private GameStates currentState;
        private GameObject player;
        private CameraController mainCamera;

        // Getters/Setters
        public GameStates CurrentState{
            get {return currentState;}
            set {
                currentState = value;

                // When the game is in a certain state, we disable the player's controller, if applicable
                switch(currentState)
                {
                    case GameStates.NORMAL:
                        player.GetComponent<CharacterController>().EnableController();
                        break;
                    case GameStates.EVENT:
                    case GameStates.TRAVEL:
                    case GameStates.MENU:
                        player.GetComponent<CharacterController>().DisableController();
                        // TODO: When creating other NPCS that move, they should pause here too
                        break;
                }
            }
        }

        // The GameManager will also manage having references to the main camera and player
        public CameraController MainCamera {
            get {return mainCamera;}
        }

        public GameObject PlayerReference {
            get {return player;}
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

        // We tell this GameObject to listen for new scene changes
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }

        // If for any reason our GameManger is disabled, we make sure we stop listening for new level changes
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnLevelFinishedLoading;
        }

		// This function is called when the GameManager detects a new scene
		private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if(scene.buildIndex != 1)
            {
                // If for some reason, the gamemanager says its in a normal mode upon loading a scene, we tell it that it is traveling.
                if(currentState == GameStates.NORMAL)
                {
                    currentState = GameStates.TRAVEL;
                }
                StartCoroutine(SetUpScene(scene.buildIndex));
            }
        }

        // Called in OnLevelFinishedLoading so that the game can load up certain things
        private IEnumerator SetUpScene(int sceneIndex)
        {
            // We first find the PlayerSpawn GameObject where we will put the player at and the main camera
            Vector3 playerSpawn = GameObject.FindWithTag("PlayerSpawn").transform.position;
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();

            // We then check if we have the player spawned in
            if(player == null)
            {
                // If we can't find the player, we spawn the player in the level
                player = Instantiate(playerPrefab, playerSpawn, Quaternion.identity);
                DontDestroyOnLoad(player);
            }
            else
            {
                // We just find the player in the game scene
                player = GameObject.FindWithTag("Player");
                player.GetComponent<CharacterController>().WarpCharacter(playerSpawn);
            }

            yield return new WaitForEndOfFrame();

            // We check to see what scene we are in. If we are in a cutscene, we do an additional step with the player
            // All cutscenes will be in a build index < 3 (for now)
            if(sceneIndex < 3)
            {
                mainCamera.objectToFollow = player.transform;

                // We fade into the scene
                GameObject.FindWithTag("FadeUI").GetComponent<Image>().CrossFadeAlpha(0, 0.5f, true);
                yield return new WaitForSeconds(0.5f);

                // We enable the player to move and the game resumes
                CurrentState = GameStates.NORMAL;
            }
            else
            {
                // If we are in a cutscene, we only fade into the scene
                GameObject.FindWithTag("FadeUI").GetComponent<Image>().CrossFadeAlpha(0, 0.5f, true);
                yield return new WaitForSeconds(0.5f);
            }
        }
	}
}