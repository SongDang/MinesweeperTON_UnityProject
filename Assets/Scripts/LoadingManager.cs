using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    void Start()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync(2);
        load.allowSceneActivation = false;

        while (!load.isDone)
        {
            float progress = Mathf.Clamp01(load.progress / 0.9f);
            progressBar.value = progress;
            progressText.text = Mathf.RoundToInt(progress * 100) + "%";

            // 100%
            if (load.progress >= 0.9f)
            {
                progressBar.value = 1;
                progressText.text = "100%";
                yield return new WaitForSeconds(3f); 
                load.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
