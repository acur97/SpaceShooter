using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class FirestoreManager : MonoBehaviour
{
    [SerializeField] private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/polygonus-spaceshooter/databases/(default)/documents/Leaderboard/Tt5KB3zN015943sAcR5Q";
    private const string _patch = "PATCH";

    [Space]
    [SerializeField] private Document leaderboard;

    [Header("UI")]
    [SerializeField] private GameObject panelEnd;
    [SerializeField] private GameObject panelSubmit;
    [SerializeField] private GameObject panelSubmiting;
    [SerializeField] private GameObject panelSubmited;
    private bool isOn = false;

    [Space]
    [SerializeField] private TMP_InputField nameInput;

    [Space]
    [SerializeField] private GameObject recordPrefab;
    [SerializeField] private Transform recordContainer;

    private const char scoreSeparator = '_';
    private const string scorePoint = ".";
    private string[] playerStats = new string[2];
    private string playerName;
    private string playerScore;

    private void Awake()
    {
        panelEnd.SetActive(true);
        panelSubmit.SetActive(false);
        panelSubmiting.SetActive(false);
        panelSubmited.SetActive(false);
    }

    private void Start()
    {
        nameInput.text = PlayerProgress.GetPlayerName();
    }

    public void SetLeaderboardStatus(bool on)
    {
        if (on && isOn)
        {
            return;
        }

        Leaderboard(on).Forget();
    }

    public async UniTaskVoid Leaderboard(bool on)
    {
        isOn = on;

        if (on)
        {
            await DownloadLeaderboard();

            SetLeaderboardRecords();
        }

        UiManager.Instance.SetUi(UiType.Leaderboard, on, 0.5f);
    }

    private async UniTask DownloadLeaderboard()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(firestoreUrl);
        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success && webRequest.isDone)
        {
            leaderboard = JsonUtility.FromJson<Document>(webRequest.downloadHandler.text);
        }
    }

    private void SetLeaderboardRecords()
    {
        for (int i = 0; i < recordContainer.childCount; i++)
        {
            Destroy(recordContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < leaderboard.fields.Scores.arrayValue.values.Count; i++)
        {
            playerStats = leaderboard.fields.Scores.arrayValue.values[i].stringValue.Split(scoreSeparator);
            playerScore = FormatNumber(playerStats[0]);
            playerName = playerStats[1];
            Instantiate(recordPrefab, recordContainer).GetComponent<RecordChild>().SetRecord((i + 1).ToString(), playerScore, playerName);
        }
    }

    private string FormatNumber(string input)
    {
        if (input.Length > 3)
        {
            input = input.Insert(1, scorePoint);
        }
        if (input.Length > 7)
        {
            input = input.Insert(4, scorePoint);
        }

        return input;
    }

    public void StartUploadScore()
    {
        if (string.IsNullOrEmpty(nameInput.text) || PlayerController.Instance.health > PlayerController.Instance._properties.health)
        {
            return;
        }

        panelEnd.SetActive(false);
        panelSubmit.SetActive(false);
        panelSubmiting.SetActive(true);
        panelSubmited.SetActive(false);

        UploadLeaderboard().Forget();
    }

    private async UniTaskVoid UploadLeaderboard()
    {
        await DownloadLeaderboard();

        for (int i = 0; i < leaderboard.fields.Scores.arrayValue.values.Count; i++)
        {
            playerScore = $"{GameManager.Instance.score}{scoreSeparator}{nameInput.text}";
            if (GameManager.Instance.score >= int.Parse(leaderboard.fields.Scores.arrayValue.values[i].stringValue.Split(scoreSeparator)[0]))
            {
                leaderboard.fields.Scores.arrayValue.values.Insert(i, new Document.Value() { stringValue = playerScore });
                break;
            }

            if (i == leaderboard.fields.Scores.arrayValue.values.Count - 1 && GameManager.Instance.score > 0)
            {
                leaderboard.fields.Scores.arrayValue.values.Add(new Document.Value() { stringValue = playerScore });
                break;
            }
        }

        using UnityWebRequest webRequest = UnityWebRequest.Put(firestoreUrl, JsonUtility.ToJson(leaderboard));
        webRequest.method = _patch;
        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success && webRequest.isDone)
        {
            panelEnd.SetActive(false);
            panelSubmit.SetActive(false);
            panelSubmiting.SetActive(false);
            panelSubmited.SetActive(true);

            PlayerProgress.SavePlayerName(nameInput.text);
        }
    }
}

[Serializable]
public class Document
{
    public Fields fields;

    [Serializable]
    public class Fields
    {
        public Scores Scores;
    }

    [Serializable]
    public class Scores
    {
        public ArrayValue arrayValue;
    }

    [Serializable]
    public class ArrayValue
    {
        public List<Value> values;
    }

    [Serializable]
    public class Value
    {
        public string stringValue;
    }
}