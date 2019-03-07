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
        TRAVEL
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

        private bool justEnteredScene;
        private float alpha;
        private Image fadeOverlay;

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

        // Fades in the scene when the player enters 
		private void Update()
		{
            if(justEnteredScene == true && fadeOverlay != null)
            {
                alpha = Mathf.Lerp(alpha, 0, 0.1f);
                Color newColor = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, alpha);
                fadeOverlay.color = newColor;
            }
		}

		// This function is called when the GameManager detects a new scene
		private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            if(scene.buildIndex != 1)
            {
                justEnteredScene = true;
                alpha = 1;

                StartCoroutine(SetUpScene());
            }
        }

        // Called in OnLevelFinishedLoading so that the game can load up certain things
        private IEnumerator SetUpScene()
        {
            Vector3 newPos = GameObject.FindWithTag("PlayerSpawn").transform.position;
            fadeOverlay = GameObject.FindWithTag("FadeUI").GetComponent<Image>();
            yield return new WaitForEndOfFrame();

            // We first check if we have the player spawned in
            if(player == null)
            {
                // If we can't find the player, we spawn the player in the level
                player = Instantiate(playerPrefab, newPos, Quaternion.identity);
                DontDestroyOnLoad(player);
            }
            else
            {
                // We just find the player in the game scene
                player = GameObject.FindWithTag("Player");
                player.GetComponent<CharacterController>().WarpPlayer(newPos);
            }
            yield return new WaitForEndOfFrame();

            // We then reassign any crucial components that are related to the player or main camera
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
            mainCamera.objectToFollow = player.transform;
            yield return new WaitForEndOfFrame();

            // If needed, we restart the player controller and make the fade effect finish
            justEnteredScene = false;
            fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, 0);
            player.GetComponent<CharacterController>().EnableController();
            CurrentState = GameStates.NORMAL;
            yield return null;
        }
	}
}