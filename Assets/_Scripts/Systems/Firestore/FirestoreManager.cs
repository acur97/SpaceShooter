using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class FirestoreManager : MonoBehaviour
{
    [SerializeField, TextArea] private string firestoreUrl = "https://firestore.googleapis.com/v1/projects/polygonus-spaceshooter/databases/(default)/documents/Leaderboard/Normal_Scores";
    [SerializeField, TextArea] private string firestoreInfiniteUrl = "https://firestore.googleapis.com/v1/projects/polygonus-spaceshooter/databases/(default)/documents/Leaderboard/Infinite_Scores";
    private string CurrentUrl => RoundsController.Instance.levelType == RoundsController.LevelType.Normal ? firestoreUrl : firestoreInfiniteUrl;
    private const string _patch = "PATCH";
    private bool downloading = false;

    [Space]
    [SerializeField] private Document leaderboard;

    [Header("Leaderboard UI")]
    [SerializeField] private GameObject recordPrefab;
    [SerializeField] private Transform recordContainer;

    [Header("Final UI")]
    [SerializeField] private GameObject panelEnd;
    [SerializeField] private GameObject panelSubmit;
    [SerializeField] private GameObject panelSubmiting;
    [SerializeField] private GameObject panelSubmited;
    private bool isOn = false;

    [Space]
    [SerializeField] private TMP_InputField nameInput;

    private const char scoreSeparator = '_';
    private const string scorePattern = @"(\d)(?=(\d{3})+$)";
    private const string scoreReplacement = "$1.";
    private string[] playerStats = new string[2];
    private string playerName;
    private string playerScore;

    private CancellationTokenSource cancellationToken;

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
        if (!on)
        {
            isOn = false;
            if (!GameManager.Instance.hasEnded)
            {
                UiManager.Instance.SetUi(UiType.Select, true, 0.5f);
            }
            else
            {
                UiManager.Instance.SetUi(UiType.End, true, 0.5f);
            }
            UiManager.Instance.SetUi(UiType.Leaderboard, false, 0.5f);
            return;
        }

        if (isOn)
        {
            return;
        }

        isOn = true;
        if (!GameManager.Instance.hasEnded)
        {
            UiManager.Instance.SetUi(UiType.Select, false, 0.5f);
        }
        else
        {
            UiManager.Instance.SetUi(UiType.End, false, 0.5f);
        }
        UiManager.Instance.SetUi(UiType.Leaderboard, true, 0.5f);
        DownloadLeaderboardVoid().Forget();
    }

    public void SetDownloadLeaderboard()
    {
        if (downloading)
        {
            return;
        }

        DownloadLeaderboardVoid().Forget();
    }

    private async UniTaskVoid DownloadLeaderboardVoid()
    {
        UiManager.Instance.SetUi(UiType.Loading, true, 0.2f);
        downloading = true;

        ClearLeaderboard();

        cancellationToken?.Cancel();
        cancellationToken = new CancellationTokenSource();
        await DownloadLeaderboard(cancellationToken);

        SetLeaderboardRecords();

        UiManager.Instance.SetUi(UiType.Loading, false, 0.2f);
        downloading = false;
    }

    private void ClearLeaderboard()
    {
        for (int i = 0; i < recordContainer.childCount; i++)
        {
            Destroy(recordContainer.GetChild(i).gameObject);
        }
    }

    private async UniTask DownloadLeaderboard(CancellationTokenSource cancellationToken)
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(CurrentUrl);
        await webRequest.SendWebRequest();

        if (!cancellationToken.IsCancellationRequested && webRequest.result == UnityWebRequest.Result.Success)
        {
            leaderboard = JsonUtility.FromJson<Document>(webRequest.downloadHandler.text);
        }
    }

    private void SetLeaderboardRecords()
    {
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
        return Regex.Replace(input, scorePattern, scoreReplacement);
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
        cancellationToken?.Cancel();
        cancellationToken = new CancellationTokenSource();
        await DownloadLeaderboard(cancellationToken);

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

        using UnityWebRequest webRequest = UnityWebRequest.Put(CurrentUrl, JsonUtility.ToJson(leaderboard));
        webRequest.method = _patch;
        await webRequest.SendWebRequest();

        if (webRequest.result == UnityWebRequest.Result.Success && webRequest.isDone)
        {
            panelEnd.SetActive(false);
            panelSubmit.SetActive(false);
            panelSubmiting.SetActive(false);
            panelSubmited.SetActive(true);

            SetLeaderboardStatus(true);

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