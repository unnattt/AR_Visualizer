using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour
{
    public Image loadingBar;
    public float fillDuration;

    void Start()
    {
        StartCoroutine(UpdateLoadingBar(1));
    }

    IEnumerator UpdateLoadingBar(int sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;
        float timer = 0f;
        loadingBar.fillAmount = 0;
        while (timer < fillDuration)
        {
            timer += Time.deltaTime;
            float fillAmount = Mathf.Clamp01(timer / fillDuration);
            loadingBar.fillAmount = fillAmount;

            yield return null;
        }
        //yield return new WaitForSeconds(fillDuration);
        //SceneManager.LoadScene(1);
        asyncLoad.allowSceneActivation = true;
    }
}
