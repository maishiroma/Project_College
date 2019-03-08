/*  Using: https://www.youtube.com/watch?v=xJQXoG3caGc and https://www.youtube.com/watch?v=YMj2qPq9CP8
 * 
 *  Making a loading screen. Allows for some extra flexibility too. Combined aspects from both tutorials to make this
 */

// LoadingScreenManager
// --------------------------------
// built by Martin Nerurkar (http://www.martin.nerurkar.de)
// for Nowhere Prophet (http://www.noprophet.com)
//
// Licensed under GNU General Public License v3.0
// http://www.gnu.org/licenses/gpl-3.0.txt

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace MattScripts {
    
    public class LoadingScreenManager : MonoBehaviour
    {
        // The scene to load. 
        public static int sceneToLoadIndex = -1;

        // IMPORTANT! This is the build index of your loading scene. You need to change this to match your actual scene index
        static int loadingSceneIndex = 1;

        [Header("Loading Visuals")]
        public Slider progressBar;              // The progress bar that SHOULD fill up when game is loading
        public Image fadeOverlay;

        [Header("Timing Settings")]
        public float waitOnLoadEnd = 0.25f;
        public float fadeDuration = 0.25f;

        [Header("Loading Settings")]
        public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
        public ThreadPriority loadThreadPriority;

        [Header("Other")]
        public AudioListener audioListener;     // If loading additive, link to the cameras audio listener, to avoid multiple active audio listeners

        // Private variables
        private AsyncOperation operation;

        // NEED TO CALL THIS METHOD IN ANOTHER SCRIPT TO USE THIS
        public static void LoadScene(int levelNum)
        {
            if(levelNum < 0)
            {
                Debug.LogError("The scene index does not exist!");
            }
            else
            {
                // Switches the game to the loading screen
                Application.backgroundLoadingPriority = ThreadPriority.High;
                sceneToLoadIndex = levelNum;
                SceneManager.LoadScene(loadingSceneIndex);
            }
        }

        // Starts up the logic for loading up the next level
        private void Start()
        {
            if(sceneToLoadIndex == -1)
            {
                Debug.LogError("Are you running this scene just as is? You need to call LoadScene from another script to have this work!");
            }
            else
            {
                fadeOverlay.gameObject.SetActive(true); // Making sure it's on so that we can crossfade Alpha
                StartCoroutine(LoadAsync(sceneToLoadIndex));
            }
        }

        // The main logic in handling the loading visuals and changing to the next screen
        private IEnumerator LoadAsync(int levelNum)
        {
            // We first enable the loading visuals
            ShowLoadingVisuals();
            yield return null;

            // We then fade the screen
            fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
            yield return new WaitForSeconds(fadeDuration);

            // We start up the loading operation
            StartOperation(levelNum);

            // And then we loop through the logic in loading the level
            float prevProgress = 0f;
            while(DoneLoading() == false)
            {
                // This is done so that we can make our progress be scaled to 0-1
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                // We check to see if we made any signifigant progress in loading. If not, we slowly increment our bar up.
                // Else, we set the value to be the progress.
                if(Mathf.Approximately(progress, prevProgress) == true)
                {
                    progressBar.value = progress;
                    prevProgress = progress;
                }
                else
                {
                    progressBar.value += 0.01f;
                }
                yield return null;
            }

            // If we made loading the next scene additive, we reenable the audio
            if(loadSceneMode == LoadSceneMode.Additive)
            {
                audioListener.enabled = false;
            }

            // We also show the completed visuals
            ShowCompletionVisuals();
            yield return new WaitForSeconds(waitOnLoadEnd);

            // We fade out the visuals
            fadeOverlay.CrossFadeAlpha(1, fadeDuration, true);
            yield return new WaitForSeconds(fadeDuration);

            // And then we finalize the load
            if(loadSceneMode != LoadSceneMode.Additive)
            {
                operation.allowSceneActivation = true;
            }
        }

        // This method takes care of starting to load up the next level
        private void StartOperation(int levelNum)
        {
            Application.backgroundLoadingPriority = loadThreadPriority;
            operation = SceneManager.LoadSceneAsync(levelNum, loadSceneMode);

            if(loadSceneMode == LoadSceneMode.Single)
            {
                operation.allowSceneActivation = false;
            }
        }

        // A helper method that helps check if we are done loading the next scene
        private bool DoneLoading()
        {
            return (loadSceneMode == LoadSceneMode.Additive && operation.isDone) || (loadSceneMode == LoadSceneMode.Single && operation.progress >= 0.9f);
        }

        // A helper method that sets up all of the loading visuals
        private void ShowLoadingVisuals()
        {
            progressBar.value = 0f;
        }

        // A helper method that completes all of the loading visuals
        private void ShowCompletionVisuals()
        {
            progressBar.value = 100f;
        }
    }
}