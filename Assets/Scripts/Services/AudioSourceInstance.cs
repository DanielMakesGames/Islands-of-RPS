using UnityEngine;

public class AudioSourceInstance : MonoBehaviour
{
    AudioSource myAudioSource;

    void Awake()
    {
        myAudioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!myAudioSource.isPlaying)
        {
            gameObject.SetActive(false);
        }
    }
}
