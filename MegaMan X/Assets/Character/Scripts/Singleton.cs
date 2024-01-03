using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static GameObject Instance;

    public void Awake()
    {
        
        if(Instance == null)
        {
            Instance = this.gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(Instance != this.gameObject)
            {
                Destroy(gameObject);
            }
        }
    }

}
