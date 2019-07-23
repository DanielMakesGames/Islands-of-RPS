using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    static SaveData instance = null;

    const string DataInitializedKey = "IsDataInitialized";
    public const string IslandKey = "Island";
    public const string IsIslandUnlockedKey = "IsIslandUnlocked";
    public const string IsSoundEffectsOnKey = "SoundEffectsOn";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            InitializeSaveData();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void InitializeSaveData()
    {
        if (GetInt(DataInitializedKey) == 0)
        {
            SetInt(DataInitializedKey, 1);

            SetInt(IslandKey, 0);
        }
    }

    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.GetInt(key, defaultValue);
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.GetString(key, defaultValue);
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(key, value);
    }

    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.GetFloat(key, defaultValue);
    }

    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }
}