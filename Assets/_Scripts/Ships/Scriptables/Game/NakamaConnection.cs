using UnityEngine;
using Nakama;
using System;
using Cysharp.Threading.Tasks;

public class NakamaConnection : MonoBehaviour
{
    [SerializeField] private string url = "https://73c2-8-242-214-187.ngrok-free.app";
    [SerializeField] private string serverKey = "defaultkey";

    [Space]
    [SerializeField] private string leaderboardId = "4ec4f126-3f9d-11e7-84ef-b7c182b36521";

    [Space]
    [SerializeField]
    private string storageValueJson = "{\"Progreso\":{\"monedas\":13,\"color\":\"FFFFFF\"}}"
;
    //private const string SessionPrefName = "nakama.Session";
    private const string DeviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";

    private IClient Client;
    private ISession Session;
    //private ISocket Socket;

    [ContextMenu("Init Connection")]
    private async UniTaskVoid Connect()
    {
        Debug.Log("Connect");

        Client = new Client(new Uri(url), serverKey);

        //CacheSessionSearch(SessionPrefName);

        await AuthenticateSessionIfNull();

        // Socket = Client.NewSocket();
        // await Socket.ConnectAsync(Session, true);

        Debug.Log("Connect finish");
    }

    //private void CacheSessionSearch(string sessionPrefName)
    //{
    //    Debug.Log("CacheSessionSearch");

    //    if (PlayerPrefs.HasKey(sessionPrefName))
    //    {
    //        ISession session = Nakama.Session.Restore(PlayerPrefs.GetString(sessionPrefName));

    //        if (!session.IsExpired)
    //        {
    //            Session = session;
    //        }
    //    }

    //    Debug.Log("CacheSessionSearch finish");
    //}

    private async UniTask AuthenticateSessionIfNull()
    {
        Debug.Log("AuthenticateSessionIfNull");

        Session ??= await Client.AuthenticateDeviceAsync(GetDeviceIdentifier());

        Debug.Log("AuthenticateSessionIfNull finish");
    }

    private string GetDeviceIdentifier()
    {
        Debug.Log("GetDeviceIdentifier");

        if (PlayerPrefs.HasKey(DeviceIdentifierPrefName))
        {
            return PlayerPrefs.GetString(DeviceIdentifierPrefName);
        }
        else
        {
            string deviceId = SystemInfo.deviceUniqueIdentifier;

            if (deviceId == SystemInfo.unsupportedIdentifier)
            {
                deviceId = Guid.NewGuid().ToString();
            }

            PlayerPrefs.SetString(DeviceIdentifierPrefName, deviceId);

            return deviceId;
        }
    }

    [ContextMenu("Test Send Leaderboard")]
    public async UniTaskVoid SendLeaderboard()
    {
        Debug.Log("SendLeaderboard");

        await Client.WriteLeaderboardRecordAsync(Session, leaderboardId, 320);

        Debug.Log("SendLeaderboard finish");
    }

    [ContextMenu("Test Get Leaderboard")]
    public async UniTaskVoid GetLeaderboard()
    {
        Debug.Log("GetLeaderboard");

        IApiLeaderboardRecordList leaderboardRecords = await Client.ListLeaderboardRecordsAsync(Session, leaderboardId);

        foreach (IApiLeaderboardRecord record in leaderboardRecords.Records)
        {
            Debug.Log(record);
        }

        Debug.Log("GetLeaderboard finish");
    }

    [ContextMenu("Test Send Storage")]
    public async UniTaskVoid SendStorage()
    {
        Debug.Log("SendStorage");

        WriteStorageObject[] objects = new[]
        {
            new WriteStorageObject
            {
                Collection = "test_progress",
                Key = "test_data",
                Value = "Test data",
                PermissionRead = 2, // 2 = Public (others can read if needed)
                PermissionWrite = 1 // 1 = Private (only the owner can write)
            },
            new WriteStorageObject
            {
                Collection = "player_progress",
                Key = "progress_data",
                Value = storageValueJson,
                PermissionRead = 2, // 2 = Public (others can read if needed)
                PermissionWrite = 1 // 1 = Private (only the owner can write)
            }
        };

        await Client.WriteStorageObjectsAsync(Session, objects);

        Debug.Log("SendStorage finish");
    }
}