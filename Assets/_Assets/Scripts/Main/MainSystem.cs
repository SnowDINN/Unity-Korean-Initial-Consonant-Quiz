using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSystem : MonoBehaviour
{
    [SerializeField] TMP_InputField uiInputConsnant;
    [SerializeField] TMP_InputField uiInputTimer;

    void Awake()
    {
        if (CustomObserver.Default != null)
        {
            CustomObserver.Default.SetConsonant(string.Empty);
            CustomObserver.Default.SetTimer(0);
        }

        uiInputConsnant.onValueChanged.AddListener(evt =>
        {
            CustomObserver.Default.SetConsonant(evt);
        });

        uiInputTimer.onValueChanged.AddListener(evt =>
        {
            CustomObserver.Default.SetTimer(Convert.ToInt32(evt));
        });

        uiInputConsnant.onSubmit.AddListener(evt =>
        {
            SceneManager.LoadSceneAsync(1);
        });

        uiInputTimer.onSubmit.AddListener(evt =>
        {
            SceneManager.LoadSceneAsync(1);
        });
    }
}