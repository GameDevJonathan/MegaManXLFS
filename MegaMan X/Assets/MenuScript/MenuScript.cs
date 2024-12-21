using System.Collections;
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
    public void NewGame()
    {
        Debug.Log("Loading New Game");        
        //SceneManager.LoadScene("Highway");
        StartCoroutine(LoadingSceneAsync());
    }

    public void Quit()
    {

#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Debug.Log("Quiting Game");
        Application.Quit();
    }

    IEnumerator LoadingSceneAsync()
    {
        _audioSource.Stop();
        LoadingBar.gameObject.SetActive(true);

        yield return null;
        
        AsyncOperation operation 
            = SceneManager.LoadSceneAsync(1);

        while (!operation.isDone)
        {            
            LoadingFillBar.fillAmount = operation.progress;
            yield return null;
        }
    }
}
