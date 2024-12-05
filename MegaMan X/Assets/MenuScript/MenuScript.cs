using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private GameObject LoadingBar;
    [SerializeField] private Image LoadingFillBar;
    //[SerializeField] private 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void NewGame(string SceneName)
    {
        Debug.Log("Loading New Game");        
        //SceneManager.LoadScene("Highway");
        StartCoroutine(LoadingSceneAsync(SceneName));
    }

    public void Quit()
    {

#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Debug.Log("Quiting Game");
        Application.Quit();
    }

    IEnumerator LoadingSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);


        _audioSource.Stop();
        LoadingBar.gameObject.SetActive(true);

        while (!operation.isDone)
        {
            float progressValue = Mathf.Clamp01(operation.progress/ 0.9f);
            LoadingFillBar.fillAmount = progressValue;
        }

        yield return null;
    }
}
