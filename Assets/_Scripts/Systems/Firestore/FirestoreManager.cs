using UnityEngine;
using Firebase;
using Firebase.Firestore;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;

public class FirestoreManager : MonoBehaviour
{
    [SerializeField] private FirestoreConnection connection;

    private FirebaseFirestore db;
    private Leaderboard leaderboard;

    private DocumentSnapshot snapshot;

    [Header("UI")]
    [SerializeField] private GameObject panelEnd;
    [SerializeField] private GameObject panelSubmit;
    [SerializeField] private GameObject panelSubmiting;
    [SerializeField] private GameObject panelSubmited;

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

        Init();
    }

    private void Init()
    {
        FirebaseApp.Create(new AppOptions()
        {
            AppId = connection.appId,
            ApiKey = connection.apiKey,
            MessageSenderId = connection.messagingSenderId,
            StorageBucket = connection.storageBucket,
            ProjectId = connection.projectId
        });
        db = FirebaseFirestore.DefaultInstance;
    }

    public void SetLeaderboardStatus(bool on)
    {
        Leaderboard(on).Forget();
    }

    public async UniTaskVoid Leaderboard(bool on)
    {
        if (on)
        {
            await DownloadLeaderboard();

            SetLeaderboardRecords();
        }

        UiManager.Instance.SetUi(UiType.Leaderboard, on, 0.5f);
    }

    private async UniTask DownloadLeaderboard()
    {
        snapshot = await db.Collection(connection.collectionName).Document(connection.documentId).GetSnapshotAsync();

        leaderboard = snapshot.ConvertTo<Leaderboard>();
    }

    private void SetLeaderboardRecords()
    {
        for (int i = 0; i < recordContainer.childCount; i++)
        {
            Destroy(recordContainer.GetChild(i).gameObject);
        }

        for (int i = 0; i < leaderboard.Scores.Count; i++)
        {
            playerStats = leaderboard.Scores[i].Split(scoreSeparator);
            playerScore = FormatNumber(playerStats[0]);
            playerName = playerStats[1];
            Instantiate(recordPrefab, recordContainer).GetComponent<RecordChild>().SetRecord((i + 1).ToString(), playerScore, playerName);
        }
    }

    private string FormatNumber(string input)
    {
        if (input.Length > 3)
        {
            input = input.Insert(3, scorePoint);
        }
        if (input.Length > 7)
        {
            input = input.Insert(7, scorePoint);
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

        for (int i = 0; i < leaderboard.Scores.Count; i++)
        {
            if (GameManager.Instance.score >= int.Parse(leaderboard.Scores[i].Split(scoreSeparator)[0]))
            {
                leaderboard.Scores.Insert(i, $"{GameManager.Instance.score}{scoreSeparator}{nameInput.text}");
                break;
            }

            if (i == leaderboard.Scores.Count - 1 && GameManager.Instance.score > 0)
            {
                leaderboard.Scores.Add($"{GameManager.Instance.score}{scoreSeparator}{nameInput.text}");
                break;
            }
        }

        await db.Collection(connection.collectionName).Document(connection.documentId).SetAsync(leaderboard);

        panelEnd.SetActive(false);
        panelSubmit.SetActive(false);
        panelSubmiting.SetActive(false);
        panelSubmited.SetActive(true);
    }
}

[FirestoreData()]
public class Leaderboard
{
    [FirestoreProperty()]
    public List<string> Scores { get; set; }
}