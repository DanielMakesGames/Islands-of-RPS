using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    static AudioManager instance;
    public static AudioManager Instance
    {
        get { return instance; }
    }

    [SerializeField] GameObject AudioSourceDontDestroy = null;

    [SerializeField] AudioClip ButtonInAudioClip = null;
    [SerializeField] AudioClip ButtonOutAudioClip = null;

    bool isSoundEffectsOn;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        isSoundEffectsOn = SaveData.GetInt(SaveData.IsSoundEffectsOnKey, 1) == 1;
    }

    void OnEnable()
    {
        Button.OnButtonPress += PlayButtonInAudioClip;
        Button.OnButtonRelease += PlayButtonOutAudioClip;
    }

    void OnDisable()
    {
        Button.OnButtonPress -= PlayButtonInAudioClip;
        Button.OnButtonRelease -= PlayButtonOutAudioClip;
    }

    void SoundEffectsButtonOnPressed()
    {
        isSoundEffectsOn = SaveData.GetInt(SaveData.IsSoundEffectsOnKey, 1) == 1;
    }

    void PlayButtonInAudioClip()
    {
        PlayAudioClip(ButtonInAudioClip);
    }

    void PlayButtonOutAudioClip()
    {
        PlayAudioClip(ButtonOutAudioClip);
    }

    void PlayAudioClip(AudioClip clip, float pitch = 1.0f, float volume = 1f)
    {
        if (!isSoundEffectsOn)
        {
            return;
        }
        if (clip == null)
        {
            print("MISSING CLIP");
            return;
        }

        if (ObjectPools.CurrentObjectPool != null)
        {
            GameObject obj = ObjectPools.CurrentObjectPool.AudioSourcePool.GetPooledObject();
            if (obj != null)
            {
                AudioSource audioSource = obj.GetComponent<AudioSource>();
                obj.SetActive(true);
                audioSource.clip = clip;
                audioSource.pitch = pitch;
                audioSource.volume = volume;
                audioSource.loop = false;
                audioSource.Play();
            }
        }
        else
        {
            PlayAudioClipDontDestroy(clip, pitch, volume);
        }
    }

    void StopAudioClip(AudioSource audioSource)
    {
        audioSource.Stop();
    }

    void PlayAudioClipDontDestroy(AudioClip clip, float pitch = 1.0f, float volume = 1f)
    {
        if (!isSoundEffectsOn)
        {
            return;
        }
        if (clip == null)
        {
            print("MISSING CLIP");
            return;
        }
        GameObject clone = Instantiate(AudioSourceDontDestroy) as GameObject;
        AudioSource audioSource = clone.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.loop = false;
        audioSource.Play();
    }
}
