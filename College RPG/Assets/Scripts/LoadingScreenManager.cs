/*	Using: https://www.youtube.com/watch?v=xJQXoG3caGc and https://www.youtube.com/watch?v=YMj2qPq9CP8
 * 
 * 	Making a loading screen. Allows for some extra flexibility too. Combined aspects from both tutorials to make this.
 * 
 * 	Read these in order to prevent the game from freezing when the loading screen is complete:
 * 	https://gamedev.stackexchange.com/questions/130180/smooth-loading-screen-between-scenes
 *  https://forum.unity.com/threads/asynchronous-loading-freeze.191358/
 *  https://forum.unity.com/threads/scenemanager-loadsceneasync-and-getting-real-loading-times.403034/
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
using TMPro;

public class LoadingScreenManager : MonoBehaviour {

	// The scene to load. 
	public static int sceneToLoadIndex = 1;

	// IMPORTANT! This is the build index of your loading scene. You need to change this to match your actual scene index
	static int loadingSceneIndex = 1;

	[Header("Loading Visuals")]
    public TextMeshProUGUI loadingText;				// Displays the now loading text
	public Slider progressBar;				// The progress bar that SHOULD fill up when game is loading
	public Image fadeOverlay;

	[Header("Timing Settings")]
	public float fadeDuration = 0.25f;
	public float waitOnLoadEnd = 0.25f;

	// Private variables
	private AsyncOperation operation;
	private Scene currentScene;

	// NEED TO CALL THIS METHOD IN ANOTHER SCRIPT TO USE THIS
	public static void LoadScene(int levelNum) 
	{				
		if(levelNum < 0)
		{
			Debug.LogError("The scene index does not exist!");
		}
		else
		{
			Application.backgroundLoadingPriority = ThreadPriority.High;
			sceneToLoadIndex = levelNum;
			SceneManager.LoadScene(loadingSceneIndex);
		}
	}

	void Start() 
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
		
	IEnumerator LoadAsync(int levelNum) 
	{
        ShowLoadingVisuals();
		yield return null; 

		// We fade into the loading screen and begin the async load
		FadeIn();
		yield return new WaitForSeconds(fadeDuration);

		StartOperation(levelNum);
		yield return null; 

		// The second load does all of the Awake() and Start() calls. This takes a while depending on how many calls there
		// are. Thus, we can't guess how long this will take.
		// We make a percentage and simply incremente it slowely to 100
		float perc = 0;
        while(!operation.isDone && progressBar.value < 0.6f)
		{
			perc = Mathf.Lerp(perc, 1f, 0.001f);
			progressBar.value = perc;
			yield return null;
		}

		ShowCompletionVisuals();

		yield return new WaitForSeconds(waitOnLoadEnd);

		FadeOut();

		yield return new WaitForSeconds(fadeDuration);

        operation.allowSceneActivation = true;
	}

	void StartOperation(int levelNum)
	{
		operation = SceneManager.LoadSceneAsync(levelNum, LoadSceneMode.Single);
		operation.allowSceneActivation = false;
	}

	void FadeIn() 
	{
		fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
	}

	void FadeOut()
	{
		fadeOverlay.CrossFadeAlpha(1, fadeDuration, true);
	}

	void ShowLoadingVisuals() 
	{
		progressBar.value = 0f;
		loadingText.text = "Loading...";
	}

	void ShowCompletionVisuals()
	{
		progressBar.value = 100f;
		loadingText.text = "Procceed!";
	}
}