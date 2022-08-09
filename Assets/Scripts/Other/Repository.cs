using UnityEngine;

public abstract class Repository
{
    public abstract void Initialize();
    protected void Save(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }
}
