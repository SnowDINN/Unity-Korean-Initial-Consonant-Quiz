using UnityEngine;

public class CustomObserver : MonoBehaviour
{
    public static CustomObserver Default;

    [HideInInspector] public string consonant = string.Empty;
    [HideInInspector] public float timer = 0;
    [HideInInspector] public float index = 1;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Setup()
    {
        var go = new GameObject("Custom Observer");
        var component = go.AddComponent<CustomObserver>();

        Default = component;
        DontDestroyOnLoad(go);
    }

    public void SetConsonant(string consonant)
    {
        this.consonant = consonant;
    }

    public void SetTimer(float timer)
    {
        this.timer = timer;
    }

    public void SetIndex(float index)
    {
        this.index = index;
    }
}