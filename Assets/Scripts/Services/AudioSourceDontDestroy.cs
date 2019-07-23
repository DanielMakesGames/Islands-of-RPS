using UnityEngine;

public class AudioSourceDontDestroy : MonoBehaviour
{
    AudioSource myAudioSource;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        myAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!myAudioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
