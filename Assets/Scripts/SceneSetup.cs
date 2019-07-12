using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    static SceneSetup instance = null;
    public static SceneSetup Instance
    {
        get { return instance; }
    }

    void Awake() 
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            DestroyImmediate(gameObject);
        }
    }
}
