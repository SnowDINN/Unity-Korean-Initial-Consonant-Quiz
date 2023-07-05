using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KoreanDictionary
{
    public int total;
    public Korean[] item;
}

public class Korean
{
    public string word;
    public Description sense;
}

public class Description
{
    public string definition;
    public string type;
}

public class GameSystem : MonoBehaviour
{
    [SerializeField] TMP_InputField uiInputAnswer;
    [SerializeField] TextMeshProUGUI uiInputDescription;

    [Header("Timer")]
    [SerializeField] Slider uiSlider;

    [Header("Failed")]
    [SerializeField] GameObject uiObjectFailed;
    [SerializeField] TextMeshProUGUI uiTextFailed;

    List<string> save = new List<string>();
    bool isPause = false;

    void Awake()
    {
        var placeholder = uiInputAnswer.placeholder.GetComponent<TextMeshProUGUI>();
        placeholder.text = CustomObserver.Default.consonant;

        uiSlider.maxValue = CustomObserver.Default.timer;
        uiSlider.value = CustomObserver.Default.timer;

        uiInputAnswer.onSubmit.AddListener(evt =>
        {
            if (evt == " ")
                return;

            isPause = true;
            if (save.Contains(evt))
            {
                uiObjectFailed.SetActive(true);
                uiTextFailed.text = "중복단어!";
                return;
            }

            StartCoroutine(LoadData(exist =>
            {
                if (!exist)
                {
                    uiObjectFailed.SetActive(true);
                    uiTextFailed.text = "없는단어!";
                    return;
                }

                isPause = false;
                uiSlider.value = CustomObserver.Default.timer;

                uiInputAnswer.text = string.Empty;
                uiInputAnswer.ActivateInputField();

                save.Add(evt);
            }));
        });

        uiSlider.onValueChanged.AddListener(evt =>
        {
            if (evt <= 0)
            {
                uiObjectFailed.SetActive(true);
                uiTextFailed.text = "시간초과!";

                isPause = true;
            }
        });

        Observable.EveryUpdate().Subscribe(_ =>
        {
            if (!isPause)
                uiSlider.value -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
                isPause = !isPause;

            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadSceneAsync(0);
        }).AddTo(gameObject);
    }

    IEnumerator LoadData(Action<bool> callback)
    {
        var uri = "https://stdict.korean.go.kr/api/search.do?certkey_no=5612&key=4B61E0D864088786949BAE7A68F5AE52&type_search=search&req_type=json&q=" + uiInputAnswer.text;
        using (UnityWebRequest www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                StartCoroutine(LoadData(callback));
            else
            {
                if (www.isDone)
                {
                    if (string.IsNullOrEmpty(www.downloadHandler.text))
                    {
                        callback(false);
                        yield break;
                    }

                    var jObject = JObject.Parse(www.downloadHandler.text);
                    var jObjectValue = $"{jObject["channel"]}";
                    var result = JsonConvert.DeserializeObject<KoreanDictionary>(jObjectValue);
                    callback(true);

                    uiInputDescription.text = result.item[0].sense.definition;
                }
            }
        }
    }
}