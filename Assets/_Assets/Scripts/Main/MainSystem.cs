using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSystem : MonoBehaviour
{
    [SerializeField] TMP_InputField uiInputConsnant;
    [SerializeField] TMP_InputField uiInputTimer;
    [SerializeField] TextMeshProUGUI uiTextCount;

    void Awake()
    {
        if (CustomObserver.Default != null)
        {
            CustomObserver.Default.SetConsonant(string.Empty);
            CustomObserver.Default.SetIndex(1);
            CustomObserver.Default.SetTimer(0);
        }

        uiInputConsnant.onValueChanged.AddListener(evt =>
        {
            if (!string.IsNullOrEmpty(evt))
                CustomObserver.Default.SetConsonant(evt);
        });

        uiInputTimer.onValueChanged.AddListener(evt =>
        {
            int isInteger = 0;
            if (int.TryParse(evt, out isInteger))
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

    public void Minus()
    {
        var count = Convert.ToInt32(uiTextCount.text);
        count -= 1;

        if (count < 1)
            return;

        CustomObserver.Default.SetIndex(count);
        uiTextCount.text = $"{count}";
    }

    public void Plus()
    {
        var count = Convert.ToInt32(uiTextCount.text);
        count += 1;

        if (count > 10)
            return;

        CustomObserver.Default.SetIndex(count);
        uiTextCount.text = $"{count}";
    }
}