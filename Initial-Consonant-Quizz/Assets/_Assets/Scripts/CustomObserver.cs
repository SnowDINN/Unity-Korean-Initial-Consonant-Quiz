using UnityEngine;

public class CustomObserver : MonoBehaviour
{
    public static CustomObserver Default;

    public string consonant = string.Empty;
    public float timer = 0;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Setup()
    {

    }

    void Awake()
    {
        Default = this;
    }

    public void Settings(string consonant, float timer)
    {
        this.consonant = consonant;
        this.timer = timer;
    }
}