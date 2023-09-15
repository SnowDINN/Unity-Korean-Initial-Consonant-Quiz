using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KoreanDictionary
{
    public Korean[] item;
}

public class Korean
{
    public Description sense;
}

public class Description
{
    public string definition;
    public string type;
}

public class GameSystem : MonoBehaviour
{
    private const string chosung = "ㄱㄲㄴㄷㄸㄹㅁㅂㅃㅅㅆㅇㅈㅉㅊㅋㅌㅍㅎ";

    private const ushort UnicodeHangeulBase = 0xAC00;
    private const ushort UnicodeHangeulLast = 0xD79F;

    [Header("main system")]
    [SerializeField] private TMP_InputField uiInputAnswer;
    [SerializeField] private TextMeshProUGUI uiInputDescription;

    [Header("Timer")]
    [SerializeField] private Slider uiSlider;

    [Header("Failed")]
    [SerializeField] private GameObject uiObjectFailed;
    [SerializeField] private TextMeshProUGUI uiTextFailed;

    private readonly List<Coroutine> coroutines = new();
    private readonly List<string> save = new();
    private bool isPause;

    private void Awake()
    {
        uiInputAnswer.ActivateInputField();

        var placeholder = uiInputAnswer.placeholder.GetComponent<TextMeshProUGUI>();
        placeholder.text = CustomObserver.Default.consonant;

        uiSlider.maxValue = CustomObserver.Default.timer;
        uiSlider.value = CustomObserver.Default.timer;

        uiInputAnswer.onValueChanged.AddListener(evt =>
        {
            if (evt.Contains(" "))
                uiInputAnswer.text = uiInputAnswer.text.Replace(" ", "");
        });

        uiInputAnswer.onSubmit.AddListener(evt =>
        {
            var Chosung = evt.Aggregate("", (current, t) => current + divide(t));
            if (Chosung != CustomObserver.Default.consonant)
            {
                failed("초성틀림");
                return;
            }

            if (save.Contains(evt))
            {
                failed("중복단어");
                return;
            }

            StartCoroutine(dictionaryEqualWebRequest(exist =>
            {
                if (!exist)
                {
                    failed("없는단어");
                    return;
                }
                
                uiSlider.value = CustomObserver.Default.timer;

                uiInputAnswer.text = string.Empty;
                uiInputAnswer.ActivateInputField();

                save.Add(evt);
            }));
        });

        uiSlider.onValueChanged.AddListener(evt =>
        {
            if (evt <= 0)
                failed("시간초과");
        });

       coroutines.Add(StartCoroutine(countdownAsync()));
       coroutines.Add(StartCoroutine(inputEventAsync()));
    }

    private void OnDestroy()
    {
        foreach (var coroutine in coroutines.Where(coroutine => coroutine != null))
            StopCoroutine(coroutine);
    }

    private IEnumerator dictionaryEqualWebRequest(Action<bool> callback)
    {
        var uri =
            "https://stdict.korean.go.kr/api/search.do?certkey_no=5612&key=4B61E0D864088786949BAE7A68F5AE52&type_search=search&req_type=json&q=" +
            uiInputAnswer.text;
        using var www = UnityWebRequest.Get(uri);
        yield return www.SendWebRequest();

        if (www.result is UnityWebRequest.Result.ConnectionError or UnityWebRequest.Result.ProtocolError)
        {
            StartCoroutine(dictionaryEqualWebRequest(callback));
        }
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

    private IEnumerator countdownAsync()
    {
        while (uiSlider.value > 0)
        {
            if (!isPause)
                uiSlider.value -= Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.Space))
                isPause = !isPause;
            
            yield return null;   
        }
    }

    private IEnumerator inputEventAsync()
    {
        while (SceneManager.GetActiveScene().buildIndex == 1)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadSceneAsync(0);

            yield return null;
        }
    }

    private char divide(char c)
    {
        var check = Convert.ToUInt16(c);
        if (check is > UnicodeHangeulLast or < UnicodeHangeulBase)
            return default;

        var Code = check - UnicodeHangeulBase;

        var JongsungCode = Code % 28;
        Code = (Code - JongsungCode) / 28;

        var JungsungCode = Code % 21;
        Code = (Code - JungsungCode) / 21;

        var ChosungCode = Code;
        var Chosung = chosung[ChosungCode];
        return Chosung;
    }

    private void failed(string message)
    {
        uiObjectFailed.SetActive(true);
        uiTextFailed.text = message;
        isPause = true;
    }
}