using UnityEngine;
using Nakama;
using System.Threading.Tasks;
using System;

public class NakamaConnection : MonoBehaviour
{
    public string url = "https://73c2-8-242-214-187.ngrok-free.app";
    public string serverKey = "defaultkey";
    public string palStorage = "{\"Progreso\":{\"monedas\":13,\"color\":\"FFFFFF\"}}"
;
    private const string SessionPrefName = "nakama.Session";
    private const string DeviceIdentifierPrefName = "nakama.deviceUniqueIdentifier";

    //[SerializeField] private string currentMatchmakingTicket;
    //[SerializeField] private string currentMatchId;

    public IClient Client;
    public ISession Session;
    //public ISocket Socket;

    [ContextMenu("Inicar")]
    public void Va()
    {
        _ = Connect();
    }

    /// <summary>
    /// Establishes a connection to the Nakama server.
    /// </summary>
    private async Task Connect()
    {
        Debug.Log("Connect");

        // Initialize the client with the server URL and key
        Client = new Client(new Uri(url), serverKey);

        // Search for a cached session
        CacheSessionSearch(SessionPrefName);

        // Authenticate the session if it's null
        await AuthenticateSessionIfNull();
        // TODO: Re-enable socket connection when needed
        // Socket = Client.NewSocket();
        // await Socket.ConnectAsync(Session, true);

        Debug.Log("Connect finish");
    }

    /// <summary>
    /// Searches for a cached session in the PlayerPrefs and restores it if found.
    /// </summary>
    /// <param name="sessionPrefName">The name of the PlayerPrefs key where the session is stored.</param>
    private void CacheSessionSearch(string sessionPrefName)
    {
        Debug.Log("CacheSessionSearch");

        // Get the cached session from PlayerPrefs
        string cacheSession = PlayerPrefs.GetString(sessionPrefName);

        // Check if a cached session was found
        if (!string.IsNullOrEmpty(cacheSession))
        {
            // Restore the session from the cached data
            ISession session = Nakama.Session.Restore(cacheSession);

            // Check if the restored session is not expired
            if (!session.IsExpired)
            {
                // Set the restored session as the current session
                Session = session;
            }
        }

        Debug.Log("CacheSessionSearch finish");
    }

    /// <summary>
    /// Authenticates the session if it's null.
    /// </summary>
    private async Task AuthenticateSessionIfNull()
    {
        Debug.Log("AuthenticateSessionIfNull");

        // Check if the session is null
        if (Session == null)
        {
            // Get or create a unique device identifier
            string deviceId = GetDeviceIdentifier();

            // Authenticate the device with the Nakama server
            Session = await Client.AuthenticateDeviceAsync(deviceId);
        }

        Debug.Log("AuthenticateSessionIfNull finish");
    }

    /// <summary>
    /// Gets or creates a unique device identifier.
    /// </summary>
    /// <returns>A unique device identifier.</returns>
    private string GetDeviceIdentifier()
    {
        // Check if a device identifier is stored in PlayerPrefs
        if (PlayerPrefs.HasKey(DeviceIdentifierPrefName))
        {
            // Return the stored device identifier
            return PlayerPrefs.GetString(DeviceIdentifierPrefName);
        }
        else
        {
            // Get the device unique identifier
            string deviceId = SystemInfo.deviceUniqueIdentifier;

            // If the device unique identifier is not supported, generate a new one
            if (deviceId == SystemInfo.unsupportedIdentifier)
            {
                deviceId = Guid.NewGuid().ToString();
            }

            // Store the device identifier in PlayerPrefs
            PlayerPrefs.SetString(DeviceIdentifierPrefName, deviceId);

            // Return the device identifier
            return deviceId;
        }
    }

    [ContextMenu("Enviar Leaderboard")]
    /// <summary>
    /// Sends a leaderboard record to the Nakama server.
    /// </summary>
    public void EnviarPalLeaderboard()
    {
        Debug.Log("EnviarPalLeaderboard");

        // Write the leaderboard record to the Nakama server
        // The record is written to the leaderboard with the specified ID and a score of 69
        Client.WriteLeaderboardRecordAsync(Session, "4ec4f126-3f9d-11e7-84ef-b7c182b36521", 69);

        Debug.Log("EnviarPalLeaderboard finish");
    }

    [ContextMenu("Retorna Leaderboard")]
    /// <summary>
    /// Retrieves and logs all leaderboard records from the Nakama server.
    /// </summary>
    public async void RotameElLeaderboard()
    {
        Debug.Log("RotameElLeaderboard");

        // Retrieve the leaderboard records from the Nakama server
        // The leaderboard ID is hardcoded for demonstration purposes
        string leaderboardId = "4ec4f126-3f9d-11e7-84ef-b7c182b36521";
        IApiLeaderboardRecordList leaderboardRecords = await Client.ListLeaderboardRecordsAsync(Session, leaderboardId);

        // Log each leaderboard record
        foreach (IApiLeaderboardRecord record in leaderboardRecords.Records)
        {
            Debug.Log(record);
        }

        Debug.Log("RotameElLeaderboard finish");
    }

    [ContextMenu("Enviar Storage")]
    /// <summary>
    /// Writes the player's progress to the Nakama storage.
    /// </summary>
    public void EncaletameEsta()
    {
        Debug.Log("EncaletameEsta");

        // Create a new WriteStorageObject to store the player's progress
        WriteStorageObject writeStorageObject = new()
        {
            // The collection name for storing player progress
            Collection = "player_progress",
            // The unique key for this record
            Key = "progress_data",
            // The value to be stored
            Value = palStorage,
            // Permission settings: public read, private write
            PermissionRead = 2, // 2 = Public (others can read if needed)
            PermissionWrite = 1 // 1 = Private (only the owner can write)
        };

        // Write the storage object to the Nakama server asynchronously
        Client.WriteStorageObjectsAsync(Session, new[] { writeStorageObject });

        Debug.Log("EncaletameEsta finish");
    }
}