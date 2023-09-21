using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSystem : MonoBehaviour
{
    [SerializeField] private TMP_InputField uiInputConsnant;
    [SerializeField] private TMP_InputField uiInputTimer;

    private void Awake()
    {
        if (CustomObserver.Default != null)
        {
            CustomObserver.Default.SetConsonant(string.Empty);
            CustomObserver.Default.SetTimer(0);
        }

        uiInputConsnant.onValueChanged.AddListener(evt =>
        {
            if (!string.IsNullOrEmpty(evt))
                CustomObserver.Default.SetConsonant(evt);
        });

        uiInputTimer.onValueChanged.AddListener(evt =>
        {
            if (int.TryParse(evt, out var isInteger))
                CustomObserver.Default.SetTimer(isInteger);
            else
                uiInputTimer.text = string.Empty;
        });

        uiInputConsnant.onSubmit.AddListener(evt =>
        {
            if (!string.IsNullOrEmpty(CustomObserver.Default.consonant) && CustomObserver.Default.timer > 0)
                SceneManager.LoadSceneAsync(1);
        });

        uiInputTimer.onSubmit.AddListener(evt =>
        {
            if (!string.IsNullOrEmpty(CustomObserver.Default.consonant) && CustomObserver.Default.timer > 0)
                SceneManager.LoadSceneAsync(1);
        });
    }
}