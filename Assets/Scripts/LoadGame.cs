using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    public string levelSelectSceneName = "MainMenu";
    public float fadeDuration = 1f;
    public Image fadePanel;
    public GameObject loadingScreenObject;
    public LoadingScreen loadingScreen;

    [Header("Loading Settings")]
    public float minimumLoadTime = 3f;
    public float artificialDelay = 2f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(LoadLevelSelectScene());
        }
    }

    private IEnumerator LoadLevelSelectScene()
    {
        // Fade to black
        yield return StartCoroutine(FadeInPanel());

        // Activate loading screen after fade is complete
        loadingScreenObject.SetActive(true);

        // Start async loading of the new scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelSelectSceneName);
        asyncLoad.allowSceneActivation = false;

        float elapsedTime = 0f;
        float progress = 0f;

        // Update loading progress
        while (!asyncLoad.isDone)
        {
            elapsedTime += Time.deltaTime;
            progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            float displayProgress = Mathf.Min(progress, elapsedTime / minimumLoadTime);
            loadingScreen.UpdateProgress(displayProgress);

            if (asyncLoad.progress >= 0.9f && elapsedTime >= minimumLoadTime)
            {
                yield return new WaitForSeconds(artificialDelay);
                loadingScreen.UpdateProgress(1f);
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    private IEnumerator FadeInPanel()
    {
        fadePanel.gameObject.SetActive(true);
        loadingScreenObject.SetActive(false);  // Ensure loading screen is hidden during fade

        float elapsedTime = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color targetColor = Color.black;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadePanel.color = Color.Lerp(startColor, targetColor, alpha);
            yield return null;
        }

        fadePanel.color = targetColor;
    }
}